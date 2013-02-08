/****************************************************************************
CONFIDENTIAL AND PROPRIETARY INFORMATION.  The entire contents of this file
is Copyright © Macromedia, Inc. 2002, All Rights Reserved.  This
document is an unpublished trade secret of Macromedia, Inc. and may not be
viewed, copied or distributed by anyone, without the specific, written
permission of Macromedia, Inc. 
****************************************************************************/

/*

	swf2html_stdio.cpp

	SWF-to-HTML converter for search engines
	stdio implementation

*/
#include "stdafx.h"
#include <stdio.h>

#include "swf2html_stdio.h"

bool Swf2HtmlConverterStdio::ConvertSwf2Html(FILE *inputFile,
											 FILE *outputFile)
{
	m_inputFile  = inputFile;
	m_outputFile = outputFile;

	bool sts = Swf2HtmlConverter::ConvertSwf2Html();
	
	m_inputFile  = NULL;
	m_outputFile = NULL;

	return sts;
}

swf_S32 Swf2HtmlConverterStdio::ReadInput(void *buffer, swf_S32 count)
{
	return fread(buffer, 1, count, m_inputFile);
}

void Swf2HtmlConverterStdio::PutString(const char *str)
{
	fputs(str, m_outputFile);
}

void Swf2HtmlConverterStdio::PutByte(swf_U8 ch)
{
	putc(ch, m_outputFile);
}

void Swf2HtmlConverterStdio::DisplayError(const char *str)
{
	fprintf(stderr, "swf2html: %s\n", str);
}

Swf2HtmlConverterStdio::Swf2HtmlConverterStdio()
{
	m_inputFile  = NULL;
	m_outputFile = NULL;
}

bool Swf2HtmlConverterStdio::ConvertSwf2Html(const char * pInput,
											 const char * pOutput)
{
	bool sts = true;
   
	FILE *inputFile  = NULL;
	FILE *outputFile = NULL;

    // Open the file for reading.
    if (pInput == NULL || (pInput[0] == '-' && pInput[1] == 0))
	{
        // redirect input from stdin
        inputFile = stdin;
    }
    else
	{
        inputFile = fopen(pInput, "rb");
    }

    // Did we open the file?
    if (inputFile == NULL) 
    {
        sts = false;
        DisplayError("error: Can't open file\n");
    }

	// redirect output to stderr!
	if (pOutput == NULL)
	{
		outputFile = stdout;
	}
	else
	{
		// open the output file for writing
		outputFile = fopen(pOutput, "w");
		// Did we open the file?
		if (outputFile == NULL) 
		{
			sts = false;
			DisplayError("error: Can't write to text file\n");
		}
	}

	if (sts)
	{
		try
		{
			sts = ConvertSwf2Html(inputFile, outputFile);
		}
		catch(...)
		{
			sts = false;
		}
	}

	if (inputFile != NULL && inputFile != stdin)
	{
		fclose(inputFile);
	}

	if (outputFile != NULL && outputFile != stdout)
	{
		fclose(outputFile);
	}

	return sts;
}
