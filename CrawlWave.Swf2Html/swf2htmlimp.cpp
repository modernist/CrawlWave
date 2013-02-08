/****************************************************************************
CONFIDENTIAL AND PROPRIETARY INFORMATION.  The entire contents of this file
is Copyright © Macromedia, Inc. 2002, All Rights Reserved.  This
document is an unpublished trade secret of Macromedia, Inc. and may not be
viewed, copied or distributed by anyone, without the specific, written
permission of Macromedia, Inc. 
****************************************************************************/

/*

	swf2html.cpp

	SWF-to-HTML converter for search engines

*/
#include "stdafx.h"
#include <string.h>
#include <ctype.h>

#include "zlib.h"
#include "stags.h"
#include "swf2html.h"

// modify these strings to add/remove extensions and prefixes that should 
// be considered as links
#define FILTER_PREFIXES "http"
#define FILTER_SUFFIXES "htm|html|swf|pdf|php|asp|aspx|txt|shtml|jsp|cfm"

#ifndef NULL
#define NULL 0
#endif

//////////////////////////////////////////////////////////////////////
// Input script object definition.
//////////////////////////////////////////////////////////////////////


char* StripPrefix( const char* str, const char* pre)
// If str begins with pre, return the first char after in str
{
	if (!str || !pre) {
		return NULL;
	}

	while (true) {
		// Map to uppercase
		unsigned char s = toupper(*str);
		unsigned char p = toupper(*pre);

		// See if the characters are not equal or we hit the end of the strings
		if (s != p || !s || !p) {
			break;
		}

		*pre++;
		*str++;
	}
	return *pre == 0 ? const_cast< char*>( str ) : 0;
}

char *StripSuffix(const char* str, const char* suffix)
{
	if (!str || !suffix) {
		return NULL;
	}

	int strLen = strlen(str);
	int suffixLen = strlen(suffix);

	if (suffixLen > strLen) {
		return NULL;
	}

	return StripPrefix(str + strLen - suffixLen, suffix);
}

/* begin UTF16 to UTF8 conversion help */
//---------------------------------------------------------------------------------------------
//					PUBLIC WIDE STRING MANIPULATION FUNCTIONS
//---------------------------------------------------------------------------------------------

// swf_U16CharToUTF8: target_ must point to a buffer at least 4 bytes in length
void Swf2HtmlConverter::swf_U16CharToUTF8(swf_U16 c, char *target_)
{
	swf_U8 *target = (swf_U8 *)target_;
	
	if (c < 0x80) 
	{
		*target++ = (swf_U8)c;
	}
	else if (c < 0x800) 
	{
		*target++ = (swf_U8)(0xC0 | c>>6);
		*target++ = (swf_U8)(0x80 | c & 0x3F);
	}
	else// if (c < 0x10000) 
	{
		*target++ = (swf_U8)(0xE0 | c>>12);
		*target++ = (swf_U8)(0x80 | c>>6 & 0x3F);
		*target++ = (swf_U8)(0x80 | c & 0x3F);
	}

	*target = 0;
}

/* end UTF16 to UTF8 conversion help */

//////////////////////////////////////////////////////////////////////
// Inline input script object methods.
//////////////////////////////////////////////////////////////////////

//
// Inlines to parse a Flash file.
//

inline void Swf2HtmlConverter::SkipBytes(int n)
{
    m_filePos += n;
}

inline swf_U8 Swf2HtmlConverter::GetByte(void) 
{
    InitBits();
    return m_fileBuf[m_filePos++];
}

inline swf_U16 Swf2HtmlConverter::GetWord(void)
{
    swf_U8* s = m_fileBuf + m_filePos;
    m_filePos += 2;
    InitBits();
    return (swf_U16) s[0] | ((swf_U16) s[1] << 8);
}

inline swf_U32 Swf2HtmlConverter::GetDWord(void)
{
    swf_U8 * s = m_fileBuf + m_filePos;
    m_filePos += 4;
    InitBits();
    return (swf_U32) s[0] | ((swf_U32) s[1] << 8) | ((swf_U32) s[2] << 16) | ((swf_U32) s [3] << 24);
}

void Swf2HtmlConverter::SetPrefixFilters(const char *filters)
{
	m_auxPrefixes = filters;
}

void Swf2HtmlConverter::SetSuffixFilters(const char *filters)
{
	m_auxSuffixes = filters;
}

bool Swf2HtmlConverter::FilterLink(const char *str,
								   const char *filters,
								   bool isSuffix)
{
    if (!filters || !*filters)
	{
        return false;
	}
    
    const char* ptr = filters;
	while (*ptr) {
		// Scan for the next '|' delimiter or end-of-string
		const char *delimiter = ptr;
		while (*delimiter && *delimiter != '|')
		{
			delimiter++;
		}
		std::string filter(ptr, delimiter-ptr);
		// Perform the comparison
		if (isSuffix)
		{
            // This is only really a link if there are characters before the last dot
			if (StripSuffix(str, filter.c_str()) && strrchr(str, '.') > str+1)
			{
				return true;
			}
		}
		else
		{
			if (StripPrefix(str, filter.c_str()))
			{
				return true;
			}
		}
		if (!*delimiter) {
			break;
		}
		ptr = delimiter+1;
	}
    
    return false;
}

bool Swf2HtmlConverter::StringIsLink(const char* str)
{
	const char *colon = strchr(str, ':');
	if (colon) {
		// If there is a colon, this could be an absolute URL.
		// - There must be a match on the prefix.
		bool prefixMatch = (FilterLink(str, FILTER_PREFIXES) ||
							FilterLink(str, m_auxPrefixes.c_str()));
		if (!prefixMatch)
		{
			return false;
		}
		// If the URL is of the form protocol://host/filename,
		// the suffix must match too.
		const char *ptr = colon;
		// Skip over slashes in http:// or file:///
		while (*++ptr == '/')
			;
		ptr = strchr(ptr, '/');
		if (ptr)
		{
		    return (FilterLink(str, FILTER_SUFFIXES, true) ||
					FilterLink(str, m_auxSuffixes.c_str(), true));
		}
		// If no file name specified, only prefix needs to match.
		return true;
	}

	// This may be a relative URL.  Make sure the suffix
	// matches.
	return (FilterLink(str, FILTER_SUFFIXES, true) ||
			FilterLink(str, m_auxSuffixes.c_str(), true));
}

void Swf2HtmlConverter::PrintChar(const char ch)
{
    switch (ch)
    {
        case '&':
            PutString("&amp;");
            break;
        case '<':
            PutString("&lt;");
            break;
        case '>':
            PutString("&gt;");
            break;
        case '"':
            PutString("&quot;");
            break;
        case 0x0d:
            PutString("<br>");
            break;
        default:
            PutByte(ch);
            break;
    }
}

void Swf2HtmlConverter::PrintString(const char* str)
{
    while (*str)
    {
        PrintChar(*str);
        str++;
    }
}

void Swf2HtmlConverter::PrintLink(const char* str)
{
    if (m_dumpLinks && StringIsLink(str))
	{
		PutString("<a href=\"");
		PrintString(str);
		PutString("\">");
        PrintString(str);
		PutString("</a>\n");
	}
}

void Swf2HtmlConverter::PrintParagraph(const char* str)
{
    if (m_dumpText) 
	{
		PutString("<p>");
        PrintString(str);
		PutString("</p>\n");
    }
}

//////////////////////////////////////////////////////////////////////
// Input script object methods.
//////////////////////////////////////////////////////////////////////

Swf2HtmlConverter::Swf2HtmlConverter()
// Class constructor.
{
    // Initialize the input pointer.
    m_fileBuf = NULL;

    // Initialize the file information.
    m_filePos = 0;
    m_fileSize = 0;
    m_fileStart = 0;
    m_fileVersion = 0;

    // Initialize the bit position and buffer.
    m_bitPos = 0;
    m_bitBuf = 0;

    // Initialize the output flags - default behavior is to dump both links and text
    m_dumpLinks = true;
    m_dumpText = true;
}

Swf2HtmlConverter::~Swf2HtmlConverter()
// Class destructor.
{
    // Free the buffer if it is there.
    if (m_fileBuf)
    {
        delete [] m_fileBuf;
        m_fileBuf = NULL;
        m_fileSize = 0;
    }

    // clean up the hashtable of font info
	GlyphInfoTable::iterator it = m_glyphInfo.begin();
	while (it != m_glyphInfo.end()) {
		delete (*it).second;
		it++;
	}
}


swf_U16 Swf2HtmlConverter::GetTag(void)
{
    // Save the start of the tag.
    m_tagStart = m_filePos;
    m_tagZero  = m_tagStart;

    // Get the combined code and length of the tag.
    swf_U16 wRawCode = GetWord();
    swf_U16 code = wRawCode;

    // The length is encoded in the tag.
    swf_U32 len = code & 0x3f;

    // Remove the length from the code.
    code = code >> 6;

    // Determine if another long word must be read to get the length.
    if (len == 0x3f)
    {
        len = (swf_U32) GetDWord();
        m_tagZero += 4;
    }

    // Determine the end position of the tag.
    m_tagEnd = m_filePos + (swf_U32) len;
    m_tagLen = (swf_U32) len;

    return code;
}


void Swf2HtmlConverter::GetRect(swf_SRECT *r)
{
    InitBits();
    int nBits = (int) GetBits(5);
    r->xmin = GetSBits(nBits);
    r->xmax = GetSBits(nBits);
    r->ymin = GetSBits(nBits);
    r->ymax = GetSBits(nBits);
}


void Swf2HtmlConverter::GetMatrix(swf_MATRIX* mat)
{
    InitBits();

    // Scale terms
    if (GetBits(1))
    {
        int nBits = (int) GetBits(5);
        mat->a = GetSBits(nBits);
        mat->d = GetSBits(nBits);
    }
    else
    {
        mat->a = mat->d = 0x00010000L;
    }

    // Rotate/skew terms
    if (GetBits(1))
    {
        int nBits = (int)GetBits(5);
        mat->b = GetSBits(nBits);
        mat->c = GetSBits(nBits);
    }
    else
    {
        mat->b = mat->c = 0;
    }

    // Translate terms
    int nBits = (int) GetBits(5);
    mat->tx = GetSBits(nBits);
    mat->ty = GetSBits(nBits);
}


void Swf2HtmlConverter::GetCxform(swf_CXFORM* cx, bool hasAlpha)
{
    InitBits();

    bool fNeedAdd = (GetBits(1) != 0);
    bool fNeedMul = (GetBits(1) != 0);

    int nBits = (int) GetBits(4);

    cx->aa = 256; cx->ab = 0;
    if (fNeedMul)
    {
        cx->ra = (swf_S16) GetSBits(nBits);
        cx->ga = (swf_S16) GetSBits(nBits);
        cx->ba = (swf_S16) GetSBits(nBits);
        if (hasAlpha) cx->aa = (swf_S16) GetSBits(nBits);
    }
    else
    {
        cx->ra = cx->ga = cx->ba = 256;
    }

    if (fNeedAdd)
    {
        cx->rb = (swf_S16) GetSBits(nBits);
        cx->gb = (swf_S16) GetSBits(nBits);
        cx->bb = (swf_S16) GetSBits(nBits);
        if (hasAlpha) cx->ab = (swf_S16) GetSBits(nBits);
    }
    else
    {
        cx->rb = cx->gb = cx->bb = 0;
    }
}

char *Swf2HtmlConverter::GetString(void)
{
    // Point to the string.
    char *str = (char *) &m_fileBuf[m_filePos];

    // Skip over the string.
    while (GetByte());
    
    return str;
}

swf_U32 Swf2HtmlConverter::GetColor(bool fWithAlpha)
{
    swf_U32 r = GetByte();
    swf_U32 g = GetByte();
    swf_U32 b = GetByte();
    swf_U32 a = 0xff;

    if (fWithAlpha)
        a = GetByte();

    return (a << 24) | (r << 16) | (g << 8) | b;
}

swf_U32 Swf2HtmlConverter::GetEventFlags( void )
{
	return ( m_fileVersion >= 6 ? GetDWord() : GetWord() );
}

void Swf2HtmlConverter::InitBits(void)
{
    // Reset the bit position and buffer.
    m_bitPos = 0;
    m_bitBuf = 0;
}


swf_S32 Swf2HtmlConverter::GetSBits(swf_S32 n)
// Get n bits from the string with sign extension.
{
    // Get the number as an unsigned value.
    swf_S32 v = (swf_S32) GetBits(n);

    // Is the number negative?
    if (v & (1L << (n - 1)))
    {
        // Yes. Extend the sign.
        v |= -1L << n;
    }

    return v;
}


swf_U32 Swf2HtmlConverter::GetBits (swf_S32 n)
// Get n bits from the stream.
{
    swf_U32 v = 0;

    while (true)
    {
        swf_S32 s = n - m_bitPos;
        if (s > 0)
        {
            // Consume the entire buffer
            v |= m_bitBuf << s;
            n -= m_bitPos;

            // Get the next buffer
            m_bitBuf = GetByte();
            m_bitPos = 8;
        }
        else
        {
            // Consume a portion of the buffer
            v |= m_bitBuf >> -s;
            m_bitPos -= n;
            m_bitBuf &= 0xff >> (8 - m_bitPos); // mask off the consumed bits

            return v;
        }
    }
}

void Swf2HtmlConverter::ParsePlaceObject2()
{
    swf_U8 flags = GetByte();
    swf_U32 depth = GetWord();

    // Get the tag if specified.
    if (flags & splaceCharacter)
    {
        swf_U32 tag = GetWord();
    }

    // Get the matrix if specified.
    if (flags & splaceMatrix)
    {
        // this one gets called
        swf_MATRIX matrix;
        GetMatrix(&matrix);
    }

    // Get the color transform if specified.
    if (flags & splaceColorTransform) 
    {
        swf_CXFORM cxform;
        GetCxform(&cxform, true);
    }        

    // Get the ratio if specified.
    if (flags & splaceRatio)
    {
        swf_U32 ratio = GetWord();
    }    
    
    // Get the instance name
    if (flags & splaceName) 
    {
        char* pszName = GetString();
    }
        
    // Get the clipdepth if specified.
    if (flags & splaceDefineClip) 
    {
        swf_U32 clipDepth = GetWord();
    }

    if (flags & splaceDefineActions)// this tells us that the sprite has actions and/or behaviors attached to it
	{
		// Remember the tag end
		swf_U32 tagEnd = m_tagEnd;

		// Read past the "local tags"
		int n = 0;
		while ( GetTag() )
		{
			n++;
			m_filePos = m_tagEnd;
		}

		// Find the combined list of handlers
		swf_U32 eventFlags = GetEventFlags();

		// Iterate over the individual handlers
		for ( swf_U32 events = GetEventFlags(); events; events = GetEventFlags() )
		{
			swf_U32 handlerSize = GetDWord();
			swf_U32 endPos = m_filePos + handlerSize;

			if ( events & kClipEventKeyPress )
			{
				handlerSize--;
				swf_U8 keyCode = GetByte();
			}

			ParseDoAction( false );
		}
	}
}

void Swf2HtmlConverter::ParseShapeStyle(bool fWithAlpha)
{
    swf_U16 i = 0;

    // Get the number of fills.
    swf_U16 nFills = GetByte();

    // Do we have a larger number?
    if (nFills == 255)
    {
        // Get the larger number.
        nFills = GetWord();
    }

    // Get each of the fill style.
    for (i = 1; i <= nFills; i++)
    {
        swf_U16 fillStyle = GetByte();

        if (fillStyle & fillGradient)
        {
            // Get the gradient matrix.
            swf_MATRIX mat;
            GetMatrix(&mat);

            // Get the number of colors.
            swf_U16 nColors = (swf_U16) GetByte();

            // Get each of the colors.
            for (swf_U16 j = 0; j < nColors; j++)
            {
                swf_U8 pos = GetByte();
                swf_U32 rgba = GetColor(fWithAlpha);
            }
        }
        else if (fillStyle & fillBits)
        {
            // Get the bitmap matrix.
            swf_U16 uBitmapID = GetWord();
            swf_MATRIX mat;
            GetMatrix(&mat);
        }
        else
        {
            // A solid color
            swf_U32 color = GetColor(fWithAlpha);
        }
    }

    // Get the number of lines.
    swf_U16 nLines = GetByte();

    // Do we have a larger number?
    if (nLines == 255)
    {
        // Get the larger number.
        nLines = GetWord();
    }

    // Get each of the line styles.
    for (i = 1; i <= nLines; i++)
    {
        swf_U16 width = GetWord();
        swf_U32 color = GetColor(fWithAlpha);
    }
}

bool Swf2HtmlConverter::ParseShapeRecord(int& xLast, int& yLast, bool fWithAlpha)
{
    // Determine if this is an edge.
    bool isEdge = (GetBits(1) != 0);

    if (!isEdge)
    {
        // Handle a state change
        swf_U16 flags = (swf_U16) GetBits(5);

        // Are we at the end?
        if (flags == 0)
        {
            return true;
        }

        // Process a move to.
        if (flags & eflagsMoveTo)
        {
            swf_U16 nBits = (swf_U16) GetBits(5);
            swf_S32 x = GetSBits(nBits);
            swf_S32 y = GetSBits(nBits);
            xLast = x;
            yLast = y;
        }
        // Get new fill info.
        if (flags & eflagsFill0)
        {
            int i = GetBits(m_nFillBits);
        }
        if (flags & eflagsFill1)
        {
            int i = GetBits(m_nFillBits);
        }
        // Get new line info
        if (flags & eflagsLine)
        {
            int i = GetBits(m_nLineBits);
        }
        // Check to get a new set of styles for a new shape layer.
        if (flags & eflagsNewStyles)
        {
            // Parse the style.
            ParseShapeStyle(fWithAlpha);

            // Reset.
            m_nFillBits = (swf_U16) GetBits(4);
            m_nLineBits = (swf_U16) GetBits(4);
        }
  
        return flags & eflagsEnd ? true : false;
    }
    else
    {
        if (GetBits(1))
        {
            // Handle a line
            swf_U16 nBits = (swf_U16) GetBits(4) + 2;   // nBits is biased by 2

            // Save the deltas
            if (GetBits(1))
            {
                // Handle a general line.
                swf_S32 x = GetSBits(nBits);
                swf_S32 y = GetSBits(nBits);
                xLast += x;
                yLast += y;
            }
            else
            {
                // Handle a vert or horiz line.
                if (GetBits(1))
                {
                    // Vertical line
                    swf_S32 y = GetSBits(nBits);
                    yLast += y;
                }
                else
                {
                    // Horizontal line
                    swf_S32 x = GetSBits(nBits);
                    xLast += x;
                }
            }
        }
        else
        {
            // Handle a curve
            swf_U16 nBits = (swf_U16) GetBits(4) + 2;   // nBits is biased by 2

            // Get the control
            swf_S32 cx = GetSBits(nBits);
            swf_S32 cy = GetSBits(nBits);
            xLast += cx;
            yLast += cy;

            // Get the anchor
            swf_S32 ax = GetSBits(nBits);
            swf_S32 ay = GetSBits(nBits);
            xLast += ax;
            yLast += ay;
        }

        return false;
    }
}

void Swf2HtmlConverter::ParseButtonRecord(swf_U32 iByte, bool fGetColorMatrix)
{
    swf_U32 iPad = iByte >> 4;
    swf_U32 iButtonStateHitTest = (iByte & 0x8);
    swf_U32 iButtonStateDown = (iByte & 0x4);
    swf_U32 iButtonStateOver = (iByte & 0x2);
    swf_U32 iButtonStateUp = (iByte & 0x1);

    swf_U32 iButtonCharacter = (swf_U32)GetWord();
    swf_U32 iButtonLayer = (swf_U32)GetWord();

    swf_MATRIX matrix;
    GetMatrix(&matrix);

    if (fGetColorMatrix)
    {
        // nCharactersInButton always seems to be one
        int nCharactersInButton = 1;

        for (int i=0; i<nCharactersInButton; i++)
        {
            swf_CXFORM cxform;
            GetCxform(&cxform, true);   // ??could be false here??
        }
    }
}

void Swf2HtmlConverter::ParseDefineButton()
{
    swf_U32 tagid = (swf_U32) GetWord();

    swf_U32 iButtonEnd = (swf_U32)GetByte();
    do
	{
        ParseButtonRecord(iButtonEnd, false);
    }
	while ((iButtonEnd = (swf_U32)GetByte()) != 0);

    // parse ACTIONRECORDs until ActionEndFlag
    ParseDoAction(false);
}

void Swf2HtmlConverter::ParseDefineButton2()
{
    swf_U32 tagid = (swf_U32) GetWord();
    swf_U32 iTrackAsMenu = (swf_U32) GetByte();

    // Get offset to first "Button2ActionCondition"
    swf_U32 iOffset = (swf_U32) GetWord();
    swf_U32 iNextAction = m_filePos + iOffset - 2;

    //
    // Parse Button Records
    //

    swf_U32 iButtonEnd = (swf_U32)GetByte();
    do
	{
        ParseButtonRecord(iButtonEnd, true);
    }
	while ((iButtonEnd = (swf_U32)GetByte()) != 0);

    //
    // Parse Button2ActionConditions
    //

    m_filePos = iNextAction;

    swf_U32 iActionOffset = 0;
    while (true)
    {
        iActionOffset = (swf_U32) GetWord();
        iNextAction  = m_filePos + iActionOffset - 2;

        swf_U32 iCondition = (swf_U32) GetWord();

        // parse ACTIONRECORDs until ActionEndFlag
        ParseDoAction(false);

        // Action Offset of zero means there's no more
        if (iActionOffset == 0)
            break;

        m_filePos = iNextAction;
    }
}

void Swf2HtmlConverter::ParseDoAction(bool fPrintTag)
{
    for (;;) 
    {
        // Handle the action
        int actionCode = GetByte();

		if (actionCode == 0)
        {
            return;
        }

        int len = 0;
        if (actionCode & sactionHasLength) 
        {
            len = GetWord();
        }        

        swf_S32 pos = m_filePos + len;

        switch ( actionCode ) 
        {
            case sactionGotoFrame:
            {
                GetWord();
                break;
            }

            case sactionGetURL:
            {
                char *url = GetString();
                PrintLink(url);
                break;
            }

            case sactionWaitForFrame:
            {
                int frame = GetWord();
                int skipCount = GetByte();
                break;
            }
            case sactionWaitForFrame2:
            {
                int skipCount = GetByte();
                break;
            }
            case sactionPush:
            {
				while ( m_filePos < (swf_U32) pos )
				{
					swf_U8 dataType = GetByte();

					switch ( dataType ) {
							
					case kPushStringType : 
						{
                            // we'll only dump this out if it's a link.
                            // PrintLink checks whether the string is 
                            // a link before dumping it out
							char *str = GetString();
                            PrintLink(str);

							break;
						}
                    case kPushFloatType :  
						{
							GetDWord();
							break;
						}
					case kPushRegisterType :  
						{
                            GetByte();
							break;
						}
                    case kPushBooleanType :  
						{
							GetByte();	
							break;
						}
                    case kPushDoubleType :   
						{
							GetDWord();
                            GetDWord();
							break;
						}
					case kPushIntegerType :   
						{
                            GetDWord();
							break;
						}
					case kPushConstant8Type :  
						{ 
                            GetByte();
							break;
						}
                    case kPushConstant16Type :  
						{	
                            GetWord();
							break;
						}
                    }
                }

                break;
            }

            case sactionJump:
            {
                swf_U16 offset = GetWord();
                break;
            }

            case sactionGetURL2:
            {
                swf_U8 flag = GetByte();
                break;
            }

            case sactionIf:
            {
                swf_U16 offset = GetWord();
                break;
            }
            case sactionGotoFrame2: 
            {
                swf_U8 stopFlag = GetByte();
                break;
            }
			
			case sactionDefineFunction:
			{
                char* functionName = GetString();
                
                // dump parameters
                swf_WORD numParams = GetWord();
                for (swf_WORD i=0; i<numParams; i++) {
                    char* paramName = GetString();
                }
                break;
			}

			case sactionConstantPool:
			{
				swf_U16 n = GetWord();
				for ( swf_U16 i = 0; i < n; i++ )
				{
                    char* constant = GetString();
					PrintLink(constant);
				}

				break;
			}
        }

        m_filePos = pos;
    }
}



void Swf2HtmlConverter::ParseDefineFont()
{
    swf_U32 iFontID = (swf_U32)GetWord();

    int iStart = m_filePos;
    int iOffset = GetWord();

    int iGlyphCount = iOffset/2;
    GlyphInfo* newGlyphInfo = new GlyphInfo();
    newGlyphInfo->iGlyphCount = iGlyphCount;
	m_glyphInfo.insert(GlyphInfoTable::value_type(iFontID, newGlyphInfo));

    int* piOffsetTable = new int[iGlyphCount];
    piOffsetTable[0] = iOffset;

	int n;
    for (n=1; n<iGlyphCount; n++)
	{
        piOffsetTable[n] = GetWord();
	}

    for (n=0; n<iGlyphCount; n++)
    {
        m_filePos = piOffsetTable[n] + iStart;

        InitBits(); // reset bit counter

        m_nFillBits = (swf_U16) GetBits(4);
        m_nLineBits = (swf_U16) GetBits(4);

        int xLast = 0;
        int yLast = 0;

        bool fAtEnd = false;

        while (!fAtEnd)
		{
            fAtEnd = ParseShapeRecord(xLast, yLast);
		}
    }

    delete [] piOffsetTable;
}


void Swf2HtmlConverter::ParseDefineFontInfo(bool isDefineFontInfo2)
{
    swf_U32 iFontID = (swf_U32) GetWord();
    int iNameLen = GetByte();

	// Skip the font name
	SkipBytes(iNameLen);

    swf_U8 flags = GetByte();

	if (isDefineFontInfo2)
	{
        GetByte();
	}

	GlyphInfoTable::iterator it = m_glyphInfo.find(iFontID);
	if (it == m_glyphInfo.end())
	{
		return;
	}
	GlyphInfo *glyphInfo = (*it).second;
    int iGlyphCount = glyphInfo->iGlyphCount;

    glyphInfo->piCodeTable = new swf_U16[iGlyphCount];
    
    for (int n=0; n < iGlyphCount; n++)
    {
        if (flags & tfontWideChars)
		{
            swf_WORD wideCode = GetWord();
            glyphInfo->piCodeTable[n] = (swf_U16)wideCode;
        }
        else
		{
            swf_BYTE code = GetByte();
            glyphInfo->piCodeTable[n] = (swf_U16)code;
        }
    }

    glyphInfo->bWideCodes = flags & tfontWideChars;
}

bool Swf2HtmlConverter::ParseTextRecord(int nGlyphBits,
								   int nAdvanceBits,
								   GlyphInfo*& glyphInfo,
								   int& indexPos,
                                   int& yOffset,
								   bool hasAlpha)
{
    swf_U8 flags = GetByte();
    if (flags == 0) return 0;

    if (flags & 0x80)
    {
        if (flags & tflagsFont)
        {
            long fontId = GetWord();

			GlyphInfoTable::iterator it = m_glyphInfo.find(fontId);
			if (it == m_glyphInfo.end())
			{
				glyphInfo = NULL;
			}
			else
			{
				glyphInfo = (*it).second;
			}
			bool success = (glyphInfo != NULL);
        }
        if (flags & tflagsColor)
        {
            int r = GetByte();
            int g = GetByte();
            int b = GetByte();

            // only in stagDefineText2
            if(hasAlpha) {
                GetByte();
			}
        }
        if (flags & tflagsX)
        {
            int iXOffset = GetWord();
        }
        if (flags & tflagsY)
        {
            int iYOffset = GetWord();
            
            // if our y offset has changed, put the upcoming text
            // on a separate line
            if(yOffset && (yOffset != iYOffset))
                PutString("<br>");
            
            yOffset = iYOffset;
        }
        if (flags & tflagsFont)
        {
            int iFontHeight = GetWord();
        }
    }
    else
    {
        int iGlyphCount = flags;
        InitBits();     // reset bit counter

        for (int g = 0; g < iGlyphCount; g++)
        {
            int iIndex = GetBits(nGlyphBits);
            int iAdvance = GetBits(nAdvanceBits);
            
            // Handle a glyph
            if ( glyphInfo && glyphInfo->piCodeTable) {
				swf_U16 code = glyphInfo->piCodeTable[iIndex];
				if (m_fileVersion >= 6)
				{
					// Write to output as UTF-8
					char buffer[4];
					swf_U16CharToUTF8(code, buffer);
					PrintString(buffer);
				}
				else
				{
					// Write directly to output
					if (code > 0xFF)
					{
						PutByte((code>>8)&0xFF);
					}
					PutByte(code&0xFF);
				}
            }
        }
        
    }

    return true;
}


void Swf2HtmlConverter::ParseDefineText(bool isDefineText2)
{
    swf_U32 tagid = (swf_U32) GetWord();

    swf_SRECT rect;
    GetRect(&rect);

    swf_MATRIX m;
    GetMatrix(&m);

    int nGlyphBits = (int)GetByte();
    int nAdvanceBits = (int)GetByte();

    bool fContinue = true;

    GlyphInfo* glyphInfo = NULL;

	GlyphInfoTable::iterator it = m_glyphInfo.find(tagid);
	if (it != m_glyphInfo.end())
	{
		glyphInfo = (*it).second;
	}
   
    int indexPos = 0;
    int yOffset = 0;

    // note that the stagDefineText2 tag indicates that the text has an
    // alpha byte stored away with the color.

    if (m_dumpText)
	{
		PutString("<p>");

		do
		{
			fContinue = ParseTextRecord(nGlyphBits, nAdvanceBits, glyphInfo, indexPos, yOffset, isDefineText2);
		}
		while (fContinue);

		PutString("</p>\n");
	}
}

void Swf2HtmlConverter::ParseDefineEditText()
{
    swf_U32 tagid = (swf_U32) GetWord();

    swf_SRECT rBounds;
    GetRect(&rBounds);

    swf_U16 flags = GetWord();

	if (flags & seditTextFlagsPassword)
	{
		return;
	}

    if (flags & seditTextFlagsHasFont)
    {
        swf_U16 uFontId = GetWord();
        swf_U16 uFontHeight = GetWord();
    }

    if (flags & seditTextFlagsHasTextColor)
    {
        GetColor(true);
    }

    if (flags & seditTextFlagsHasMaxLength)
    {
        int iMaxLength = GetWord();
    }

    if (flags & seditTextFlagsHasLayout)
    {
        int iAlign = GetByte();
        swf_U16 uLeftMargin = GetWord();
        swf_U16 uRightMargin = GetWord();
        swf_U16 uIndent = GetWord();
        swf_U16 uLeading = GetWord();
    }

    char* pszVariable = GetString();

    if (flags & seditTextFlagsHasText)
    {
        char* pszInitialText = GetString();
        if (flags & seditTextFlagsHTML)
		{
            PutString(pszInitialText);
			PutByte('\n');
		}
        else
        {
            PrintParagraph(pszInitialText);
        }
    }
}

void Swf2HtmlConverter::ParseDefineFont2()
{
    swf_U32 tagid = (swf_U32) GetWord();

    swf_U16 flags = GetWord();

    // Skip the font name
    int iNameLen = GetByte();

	SkipBytes(iNameLen);
    
    // Get the number of glyphs.
    swf_U16 nGlyphs = GetWord();

    int iDataPos = m_filePos;

    if (nGlyphs > 0)
    {
        //
        // Get the FontOffsetTable
        //

        swf_U32* puOffsetTable = new swf_U32[nGlyphs];
        int n;
        for (n=0; n<nGlyphs; n++)
		{
            if (flags & sfontFlagsWideOffsets)
			{
                puOffsetTable[n] = GetDWord();
			}
            else
			{
                puOffsetTable[n] = GetWord();
			}
		}

        //
        // Get the CodeOffset
        //

        swf_U32 iCodeOffset = 0;
        if (flags & sfontFlagsWideOffsets)
		{
            iCodeOffset = GetDWord();
		}
        else
		{
            iCodeOffset = GetWord();
		}

        //
        // Get the Glyphs
        //

        for (n=0; n<nGlyphs; n++)
        {
            m_filePos = iDataPos + puOffsetTable[n];

            InitBits(); // reset bit counter

            m_nFillBits = (swf_U16) GetBits(4);
            m_nLineBits = (swf_U16) GetBits(4);

            int xLast = 0;
            int yLast = 0;

            bool fAtEnd = false;

            while (!fAtEnd)
			{
                fAtEnd = ParseShapeRecord(xLast, yLast);
			}
        }

        delete [] puOffsetTable;

        if (m_filePos != iDataPos + iCodeOffset)
        {
            return;
        }
        
        //
        // Get the CodeTable
        //

        GlyphInfo* glyphInfo = new GlyphInfo();
        glyphInfo->iGlyphCount = nGlyphs;

		m_glyphInfo.insert(GlyphInfoTable::value_type(tagid, glyphInfo));

		glyphInfo->piCodeTable = new swf_U16[nGlyphs];
        
        m_filePos = iDataPos + iCodeOffset;

        for (n=0; n < nGlyphs; n++)
        {
            if (flags & sfontFlagsWideCodes) {
                swf_WORD wideCode = GetWord();
                glyphInfo->piCodeTable[n] = (swf_U16)wideCode;
            }
            else {
                swf_BYTE code = GetByte();
                glyphInfo->piCodeTable[n] = (swf_U16)code;
            }
        }

        glyphInfo->bWideCodes = ((flags & sfontFlagsWideCodes) != 0);
    }

    if (flags & sfontFlagsHasLayout)
    {
        //
        // Get "layout" fields
        //

        swf_S16 iAscent = GetWord();
        swf_S16 iDescent = GetWord();
        swf_S16 iLeading = GetWord();

        // Skip Advance table
        SkipBytes(nGlyphs * 2);

        // Get BoundsTable
        int i;
        for (i=0; i<nGlyphs; i++)
        {
            swf_SRECT rBounds;
            GetRect(&rBounds);
        }

        //
        // Get Kerning Pairs
        //

        swf_S16 iKerningCount = GetWord();

        for (i=0; i<iKerningCount; i++)
        {
            swf_U16 iCode1, iCode2;
            if (flags & sfontFlagsWideCodes)
            {
                iCode1 = GetWord();
                iCode2 = GetWord();
            }
            else
            {
                iCode1 = GetByte();
                iCode2 = GetByte();
            }
            swf_S16 iAdjust = GetWord();
        }
    }
}

void Swf2HtmlConverter::ParseDefineText2()
{
    ParseDefineText(true);
}

void Swf2HtmlConverter::ParseDoInitAction()
{
    swf_WORD tag = GetWord();
    ParseDoAction(false);
}

void Swf2HtmlConverter::ParseTags(bool sprite, swf_U32 tabs)
// Parses the tags within the file.
{
    if (sprite)
    {
        swf_U32 tagid = (swf_U32) GetWord();
        swf_U32 frameCount = (swf_U32) GetWord();
    }
    else
    {        
        // Set the position to the start position.
        m_filePos = m_fileStart;
    }        
    
    // Initialize the end of frame flag.
    bool atEnd = false;

    // Reset the frame position.
    swf_U32 frame = 0;

    // Loop through each tag.
    while (!atEnd)
    {
        // Get the current tag.
        swf_U16 code = GetTag();

        // Get the tag ending position.
        swf_U32 tagEnd = m_tagEnd;

        switch (code)
        {
            case stagEnd:
                // We reached the end of the file.
                atEnd = true;
                break;
        
            case stagPlaceObject2:
                ParsePlaceObject2();
                break;

            case stagDoAction:
                ParseDoAction();
                break;

            case stagDefineButton:
                ParseDefineButton();
                break;

            case stagDefineButton2:
                ParseDefineButton2();
                break;

            case stagDefineFont:
                ParseDefineFont();
                break;

            case stagDefineFontInfo:
                ParseDefineFontInfo(false);
                break;

            case stagDefineText:
                ParseDefineText();
                break;

            case stagDefineText2:
                ParseDefineText2();
                break;

            case stagDefineEditText:
                ParseDefineEditText();
                break;

            case stagDefineSprite:
                ParseTags(true, tabs + 1);
                break;

            case stagDefineFont2:
                ParseDefineFont2();
                break;

			case stagDefineFontInfo2:
				ParseDefineFontInfo(true);
				break;

            case stagDoInitAction:
                ParseDoInitAction();
                break;
        }

        // Increment the past the tag.
        m_filePos = tagEnd;
    }
}

bool Swf2HtmlConverter::ConvertSwf2Html()
{
    swf_U8 fileHdr[8];
    bool sts = true;
	bool bCompressed = false; // true if this is a Flash 6 compressed file

    // Free the buffer if it is there.
    if (m_fileBuf != NULL)
    {
        delete [] m_fileBuf;
        m_fileBuf = NULL;
        m_fileSize = 0;
    }

    if (sts)
    {
        // Read the file header.
        if (ReadInput(fileHdr, 8) != 8)
        {
            sts = false;
            DisplayError("error: Can't read the header of file\n");
        }
    }

    if (sts)
    {
        // Verify the header and get the file size.
        if (fileHdr[0] != 'F' || fileHdr[1] != 'W' || fileHdr[2] != 'S' )
        {
            if ( fileHdr[0] != 'C' || fileHdr[1] != 'W' || fileHdr[2] != 'S' ) 
			{
				DisplayError("error: Illegal Header - not a Macromedia Flash File\n");

				// Bad header.
				sts = false;
			}
			else
			{
				// Compressed Flash 6 SWF
				bCompressed = true;
				// Get the file version.
				m_fileVersion = (swf_U16) fileHdr[3];
			}
        }
        else
        {
            // Get the file version.
            m_fileVersion = (swf_U16) fileHdr[3];
        }
    }
        
    if (sts)
    {
        // Get the file size.
        m_fileSize =	(swf_U32) fileHdr[4] | ((swf_U32) fileHdr[5] << 8) | 
						((swf_U32) fileHdr[6] << 16) | ((swf_U32) fileHdr[7] << 24);

        // Verify the minimum length of a Flash file.
        if (m_fileSize < 8)
        {
            DisplayError("error: file size is too short\n");
            // The file is too short.
            sts = false;
        }
    }

    if (sts)
    {
        // Allocate the buffer.
        m_fileBuf = new swf_U8[m_fileSize];

        // Is there data in the file?
        if (m_fileBuf == NULL)
        {
            sts = false;
        }
    }
        
    if (sts)
    {
        // Copy the data already read from the file.
        memcpy(m_fileBuf, fileHdr, 8);

		if ( bCompressed )
		{
			// Allocate the buffer.
			swf_U8* expandBuf = new swf_U8[m_fileSize - 8];

			// Read the compressed file into the expansion buffer
			swf_S32 chunkLen = ReadInput(expandBuf, m_fileSize - 8);
			
			// Inflate the file into the read buffer.

			uLongf destLen = m_fileSize-8;
			if (uncompress(m_fileBuf+8, &destLen, expandBuf, chunkLen) != Z_OK)
			{
				sts = false;
			}

			// Free the decompression buffer if it is there.
			if (expandBuf)
			{
				delete [] expandBuf;
				expandBuf = NULL;
			}
		}
		else
		{
			// Read the file straight into the read buffer.
			if (ReadInput(&m_fileBuf[8], m_fileSize - 8) != (m_fileSize - 8))
			{
				sts = false;
			}
		}
    }

    if (sts)
    {
        swf_SRECT rect;
        
        // Set the file position past the header and size information.
        m_filePos = 8;

        // Get the frame information.
        GetRect(&rect);
        
        swf_U32 frameRate = GetWord() >> 8;
        swf_U32 frameCount = GetWord();

        // Set the start position.
        m_fileStart = m_filePos;    

        // if we're dumping text, print the html header
        PutString("<html>\n");
        PutString("<head>\n");
        if (m_fileVersion >= 6) {
			// Output UTF-8 character set tag for Flash 6 and later.
			PutString("<meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\">\n");
		}
        PutString("<title></title>\n");
        PutString("</head>\n");
        PutString("<body>\n");

        // Parse the tags within the file.
        ParseTags(false, 0);

        PutString("</body>\n");
        PutString("</html>\n");
	}

    // Free the buffer if it is there.
    if (m_fileBuf != NULL)
    {
        delete [] m_fileBuf;
        m_fileBuf = NULL;
    }

    // Reset the file information.
    m_filePos = 0;
    m_fileSize = 0;
    m_fileStart = 0;
    m_fileVersion = 0;

    // Reset the bit position and buffer.
    m_bitPos = 0;
    m_bitBuf = 0;

    return sts;
}

