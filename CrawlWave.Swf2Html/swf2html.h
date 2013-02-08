/****************************************************************************
CONFIDENTIAL AND PROPRIETARY INFORMATION.  The entire contents of this file
is Copyright © Macromedia, Inc. 2002, All Rights Reserved.  This
document is an unpublished trade secret of Macromedia, Inc. and may not be
viewed, copied or distributed by anyone, without the specific, written
permission of Macromedia, Inc. 
****************************************************************************/

/*

	swf2html.h

	SWF-to-HTML converter for search engines

*/

#ifndef __SWF2HTML_H
#define __SWF2HTML_H

#ifdef _MSC_VER
#pragma warning( disable : 4786 34 )
#endif

#include <string>
#include <map>

typedef unsigned long       swf_DWORD;
typedef unsigned char       swf_BYTE;
typedef unsigned short      swf_WORD;

typedef unsigned long		swf_U32;
typedef signed long			swf_S32;
typedef unsigned short		swf_U16;
typedef signed short		swf_S16;
typedef unsigned char		swf_U8;
typedef signed char			swf_S8;

class Swf2HtmlConverter  
{
public:
    // Constructor/destructor.
    Swf2HtmlConverter();
    ~Swf2HtmlConverter();

	bool ConvertSwf2Html();

	bool GetDumpLinks() const { return m_dumpLinks; }
	void SetDumpLinks(bool dumpLinks) { m_dumpLinks = dumpLinks; }
	bool GetDumpText() const { return m_dumpText; }
	void SetDumpText(bool dumpText) { m_dumpText = dumpText; }

	const char *GetPrefixFilters() const { return m_auxPrefixes.c_str(); }
	void SetPrefixFilters(const char *filters);
	const char *GetSuffixFilters() const { return m_auxSuffixes.c_str(); }
	void SetSuffixFilters(const char *filters);

protected:
	virtual void PutByte(swf_U8 ch) = 0;
	virtual void PutString(const char *str) = 0;
	virtual swf_S32 ReadInput(void *buffer, swf_S32 count) = 0;
	virtual void DisplayError(const char *str) = 0;

private:
	typedef swf_S32 swf_SFIXED;	// a 16.16 fixed point number
	typedef swf_S32 swf_SCOORD;

	struct swf_CXFORM
	{
		swf_S16 aa, ab;     // a is multiply factor, b is addition factor
		swf_S16 ra, rb;
		swf_S16 ga, gb;
		swf_S16 ba, bb;
	};

	struct swf_SRECT
	{
		swf_SCOORD xmin;
		swf_SCOORD xmax;
		swf_SCOORD ymin;
		swf_SCOORD ymax;
	};

	struct swf_MATRIX
	{
		swf_SFIXED a;
		swf_SFIXED b;
		swf_SFIXED c;
		swf_SFIXED d;
		swf_SCOORD tx;
		swf_SCOORD ty;
	};

	struct GlyphInfo
	{
		GlyphInfo() {
			bWideCodes = false;
			iGlyphCount = 0;
			piCodeTable = NULL;
		}
		~GlyphInfo() {
			if (piCodeTable) {
				delete [] piCodeTable;
			}
		}

		bool bWideCodes;
		int iGlyphCount;
		swf_U16* piCodeTable;
	};

	typedef std::map<swf_U16, GlyphInfo*> GlyphInfoTable;

    // Pointer to file contents buffer.
    swf_U8 *m_fileBuf;

    // File state information.
    swf_U32 m_filePos;
    swf_U32 m_fileSize;
    swf_U32 m_fileStart;
    swf_U16 m_fileVersion;

    // Command line prefix/suffix filters for links
	std::string m_auxPrefixes;
	std::string m_auxSuffixes;

    // Bit Handling
    swf_S32 m_bitPos;
    swf_U32 m_bitBuf;

    // Tag parsing information.
    swf_U32 m_tagStart;
    swf_U32 m_tagZero;
    swf_U32 m_tagEnd;
    swf_U32 m_tagLen;
    
    // Parsing information.
    swf_S32 m_nFillBits;
    swf_S32 m_nLineBits;

    // Font glyph counts (gotta save it somewhere!)
    // rsun 05.08.02 use a hashtable here, so we can
    // store font data as well (stores GlyphInfo structs)
	GlyphInfoTable m_glyphInfo;

    // Output flags
    bool m_dumpLinks;
    bool m_dumpText;

	void swf_U16CharToUTF8(swf_U16 c, char *target_);

    // Tag scanning methods.
    swf_U16 GetTag();
    void SkipBytes(int n);
    swf_U8 GetByte();
    swf_U16 GetWord();
    swf_U32 GetDWord();
    void GetRect(swf_SRECT *r);
    void GetMatrix(swf_MATRIX *matrix);
    void GetCxform(swf_CXFORM *cxform, bool hasAlpha);
    char *GetString();
    swf_U32 GetColor(bool fWithAlpha=false);
	swf_U32 GetEventFlags( void );

    bool FilterLink(const char* str, const char* filters, bool isSuffix = false);
    bool StringIsLink(const char* str);

    void PrintChar(const char ch);
    void PrintString(const char* str);
    void PrintLink(const char* str);
    void PrintParagraph(const char* str);

    // Routines for reading arbitrary sized bit fields from the stream.
    // Always call start bits before gettings bits and do not intermix 
    // these calls with GetByte, etc... 
    void InitBits();
    swf_S32 GetSBits(swf_S32 n);
    swf_U32 GetBits(swf_S32 n);

    // Tag subcomponent parsing methods
    // For shapes
    void ParseShapeStyle(bool fWithAlpha=false);
    bool ParseShapeRecord(int& xLast, int& yLast, bool fWithAlpha=false);
    void ParseButtonRecord(swf_U32 byte, bool fGetColorMatrix=true);
    bool ParseTextRecord(int nGlyphBits, int nAdvanceBits, GlyphInfo*& glyphInfo, int& indexPos, int& yOffset, bool hasAlpha = false);

    // Parsing methods.
    void ParseDefineButton();       //x 07: stagDefineButton
    void ParseDefineFont();         //x 10: stagDefineFont
    void ParseDefineText(bool isDefineText2 = false);         //x 11: stagDefineText
    void ParseDoAction(bool fPrintTag=true);                          // 12: stagDoAction    
    void ParseDefineFontInfo(bool isDefineFontInfo2);     //x 13: stagDefineFontInfo; x 62: stagDefineFontInfo2
    void ParsePlaceObject2();                      // 26: stagPlaceObject2
    void ParseDefineText2();        //x 33: stagDefineText2
    void ParseDefineButton2();      //x 34: stagDefineButton2
    void ParseDefineEditText();                    // 37: stagDefineEditText
    void ParseDefineFont2();        //x 48: stagDefineFont2
    void ParseDoInitAction();       //x 59: stagDoInitAction

    void ParseTags(bool sprite, swf_U32 tabs);
};

#endif /* __SWF2HTML_H */
