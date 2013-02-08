/****************************************************************************
CONFIDENTIAL AND PROPRIETARY INFORMATION.  The entire contents of this file
is Copyright © Macromedia, Inc. 2002, All Rights Reserved.  This
document is an unpublished trade secret of Macromedia, Inc. and may not be
viewed, copied or distributed by anyone, without the specific, written
permission of Macromedia, Inc. 
****************************************************************************/

/*

	swf2html_stdio.h

	SWF-to-HTML converter for search engines
	stdio implementation

*/

#ifndef __SWF2HTML_STDIO_H
#define __SWF2HTML_STDIO_H

#include "swf2html.h"

class Swf2HtmlConverterStdio : public Swf2HtmlConverter
{
public:
	Swf2HtmlConverterStdio();

	bool ConvertSwf2Html(FILE *inputFile,
					     FILE *outputFile);
	bool ConvertSwf2Html(const char *inputFile,
						 const char *outputFile);

protected:
	virtual void PutByte(swf_U8 ch);
	virtual void PutString(const char *str);
	virtual swf_S32 ReadInput(void *buffer, swf_S32 count);
	virtual void DisplayError(const char *str);

private:
	FILE *m_inputFile;
	FILE *m_outputFile;
};

#endif /* __SWF2HTML_STDIO_H */
