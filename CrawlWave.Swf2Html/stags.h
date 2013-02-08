/****************************************************************************
CONFIDENTIAL AND PROPRIETARY INFORMATION.  The entire contents of this file
is Copyright © Macromedia, Inc. 2002, All Rights Reserved.  This
document is an unpublished trade secret of Macromedia, Inc. and may not be
viewed, copied or distributed by anyone, without the specific, written
permission of Macromedia, Inc. 
****************************************************************************/

#ifndef STAGS_INCLUDED
#define STAGS_INCLUDED

// Tags - the high 10 bits is the op code, the low 6 bits is the length
// if the length == 0x5f, the length is indicated by the following DWORD
enum {
		stagEnd 				= 0,
		stagDefineButton 		= 7, // up obj, down obj, action (URL, Page, ???)
		stagDefineFont			= 10,
		stagDefineText			= 11,
		stagDoAction			= 12,
		stagDefineFontInfo		= 13,

		// Flash 3 tags
		stagPlaceObject2		= 26,	// the new style place w/ alpha color transform and name
		stagDefineText2			= 33,	// a text V2 includes alpha values
		stagDefineButton2		= 34,	// a Flash 3 button that contains color transform and sound info
		stagDefineSprite		= 39,	// define a sequence of tags that describe the behavior of a sprite
		stagDefineTextFormat	= 42,	// define the contents of a text block with formating information
		stagDefineFont2			= 48,	// a tag command for the Flash Generator Font information

		// Flash 4 tags
		stagDefineEditText		= 37,	// an edit text object (bounds, width, font, variable name)
        
        // Flash 5 tags
        
		// Flash 6 tags
		stagDoInitAction        = 59,
		stagDefineFontInfo2		= 62,	// Just like a font info, except it adds a language tag.
	};

// Flags for defining a shape character
enum {
		// These flag codes are used for state changes - and as return values from ShapeParser::GetEdge()
		eflagsMoveTo	   = 0x01,
		eflagsFill0	   	   = 0x02,
		eflagsFill1		   = 0x04,
		eflagsLine		   = 0x08,
		eflagsNewStyles	   = 0x10,

		eflagsEnd 	   	   = 0x80  // a state change with no change marks the end
};

// Flags for text chunks
enum {
		// These flag codes are used for state changes
		tflagsX			= 0x01,
		tflagsY			= 0x02,
		tflagsColor		= 0x04,
		tflagsFont		= 0x08,
		tflagsHeight	= 0x08
};

// Font style flags
enum {
		// Font style options
		tfontWideChars	= 0x01,
		tfontBold		= 0x02,
		tfontItalic		= 0x04,

		// Font style options
		tfontANSI		= 0x10,
		tfontShiftJIS	= 0x20,
		tfontUnicode	= 0x30
};

// Action codes
enum {
	// Flash 1 and 2 actions
	sactionHasLength	= 0x80,
	sactionGotoFrame	= 0x81,	// frame num (WORD)
	sactionGetURL		= 0x83,	// url (STR), window (STR)
	sactionWaitForFrame	= 0x8A,	// frame needed (WORD), actions to skip (BYTE)

	// Flash 4 Actions
	sactionPush			= 0x96, // type (BYTE), value (STRING or FLOAT)
	sactionJump			= 0x99, // offset (WORD)
	sactionIf			= 0x9D, // offset (WORD) Stack IN: bool
	sactionGetURL2		= 0x9A, // method (BYTE) Stack IN: url, window
	sactionGotoFrame2	= 0x9F, // flags (BYTE) Stack IN: frame
	sactionWaitForFrame2= 0x8D, // skipCount (BYTE) Stack IN: frame
    
	// Flash 5 actions
	sactionDefineFunction= 0x9B, // name (STRING), body (BYTES)
	sactionConstantPool  = 0x88, // Attaches constant pool
};

enum
{
    kPushStringType     = 0,
    kPushFloatType      = 1,
    kPushNullType       = 2,
	kPushUndefinedType  = 3,
	kPushRegisterType   = 4,
	kPushBooleanType    = 5,
	kPushDoubleType     = 6,
	kPushIntegerType    = 7,
	kPushConstant8Type  = 8,
	kPushConstant16Type = 9
};

// PlaceObject 2 flags
// format
//   depth (word)
//   character tag (word)
//   matrix
//   color transform w/alpha
//	 blend ratio (word)
//	 name (string)
enum {
	splaceMove			      = 0x01, // this place moves an exisiting object
	splaceCharacter		      = 0x02, // there is a character tag	(if no tag, must be a move)
	splaceMatrix		      = 0x04, // there is a matrix
	splaceColorTransform      = 0x08, // there is a color transform
	splaceRatio			      = 0x10, // there is a blend ratio
	splaceName			      = 0x20, // there is an object name
	splaceDefineClip	      = 0x40, // this shape should open or close a clipping bracket (character != 0 to open, character == 0 to close)
    splaceDefineActions       = 0x80, // this tells us that the sprite has actions and/or behaviors attached to it

    // this is an enternal flag that we set for placing cloned sprites this flag is not part of the file .swf format
    splaceCloneExternalSprite = 0x100  // cloning a movie which was loaded externally
};

// Template Text Flags
enum {
	sfontFlagsBold			= 0x01,
	sfontFlagsItalic		= 0x02,
	sfontFlagsWideCodes		= 0x04,
	sfontFlagsWideOffsets	= 0x08,
	sfontFlagsANSI			= 0x10,
	sfontFlagsUnicode		= 0x20,
	sfontFlagsShiftJIS		= 0x40,
	sfontFlagsHasLayout		= 0x80
};

// Language enumeration. (The high byte of the text flags.)
enum {
	sLanguageMixed				= 0,	// Flash 5 format: either Shift-J or latin-1 depending on other factors.
										// Should never occur in a Flash 6 file.
	sLanguageLatin				= 1,	// The western languages covered by latin-1: English, French, German, etc.
	sLanguageJapanese			= 2,
	sLanguageKorean				= 3,
	sLanguageSimplifiedChinese	= 4,
	sLanguageTraditionalChinese = 5,
};

// Edit Text Flags
enum {
	seditTextFlagsHasFont		= 0x0001,
	seditTextFlagsHasMaxLength	= 0x0002,
	seditTextFlagsHasTextColor	= 0x0004,
	seditTextFlagsReadOnly		= 0x0008,
	seditTextFlagsPassword		= 0x0010,
	seditTextFlagsMultiline		= 0x0020,
	seditTextFlagsWordWrap		= 0x0040,
	seditTextFlagsHasText		= 0x0080,
	seditTextFlagsUseOutlines	= 0x0100,
	seditTextFlagsHTML          = 0x0200,
	seditTextFlagsBorder		= 0x0800,
	seditTextFlagsNoSelect		= 0x1000,
	seditTextFlagsHasLayout		= 0x2000
	, /* for line above */
	seditTextFlagsAutoSize      = 0x4000
};

enum {
	kClipEventLoad	         = 0x0001,
	kClipEventEnterFrame	 = 0x0002,
	kClipEventUnload	     = 0x0004,
	kClipEventMouseMove		 = 0x0008,
	kClipEventMouseDown		 = 0x0010,
	kClipEventMouseUp		 = 0x0020,
	kClipEventKeyDown		 = 0x0040,
	kClipEventKeyUp			 = 0x0080,
    kClipEventData           = 0x0100
	, /* for line above */
	kClipEventInitialize     = 0x00200,
	kClipEventPress          = 0x00400,
	kClipEventRelease        = 0x00800,
	kClipEventReleaseOutside = 0x01000,
	kClipEventRollOver       = 0x02000,
	kClipEventRollOut        = 0x04000,
	kClipEventDragOver       = 0x08000,
	kClipEventDragOut        = 0x10000,
	kClipEventKeyPress       = 0x20000
};

// NOTE: this was pulled from shape.h
enum { 	// Fill Styles
		fillSolid 				= 0, 

		// Gradient fills
		fillGradient			= 0x10, // if this bit is set, must be a gradient fill
		fillLinearGradient 		= 0x10, 
		fillRadialGradient 		= 0x12, 

		fillMaxGradientColors 	= 8,

		// Vector Pattern fills
		fillVectorPattern		= 0x20,	// if this bit is set, must be a vector pattern
		fillRaggedCrossHatch 	= 0x20,
		fillDiagonalLines 		= 0x21,
		fillCrossHatchLines 	= 0x22,
		fillStipple 			= 0x23,

		// Texture/bitmap fills - note these are currently only used by our player
		fillBits				= 0x40,	// if this bit is set, must be a bitmap pattern
		fillBitsClip			= 0x01,	// flag that a bitmap should be clipped to its edges, otherwise a repeating texture
		fillBitsNoSmooth		= 0x02,	// set if a bitmap should never be smoothed

		// Fill bit Flags
		fillLooseClip			= 0x1,
		fillOverlapClip			= 0x2
	};

#endif // STAGS_INCLUDED
