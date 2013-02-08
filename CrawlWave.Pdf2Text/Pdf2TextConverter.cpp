// Pdf2TextConverter.cpp : Implementation of CPdf2TextConverter

#include "stdafx.h"
#include "Pdf2TextConverter.h"
#include ".\pdf2textconverter.h"


// CPdf2TextConverter


STDMETHODIMP CPdf2TextConverter::ConvertPdf2Text(BSTR inputFile, BSTR outputFile, LONG* success)
{
	USES_CONVERSION;
	char *input, *output;
	long retVal;

	input=new char[SysStringLen(inputFile)*2];
	output=new char[SysStringLen(outputFile)*2];
	unsigned int i=0;
	for(i=0; i<SysStringLen(inputFile)*2; i++)
	{
		input[i]='\0';
	}
	for(i=0; i<SysStringLen(outputFile)*2; i++)
	{
		output[i]='\0';
	}
	WideCharToMultiByte(1253,0,inputFile,SysStringLen(inputFile),input,SysStringLen(inputFile),NULL,NULL);
	WideCharToMultiByte(1253,0,outputFile,SysStringLen(outputFile),output,SysStringLen(outputFile),NULL,NULL);
	
	retVal = PerformConversion(input, output);

	*success = retVal;

	return S_OK;
}

int CPdf2TextConverter::PerformConversion(char *in, char *out)
{
	PDFDoc *doc;
	GString *fileName;
	TextOutputDev *textOut;
	UnicodeMap *uMap;
	int exitCode = 99;

	fileName = new GString(in);

	// get mapping to output encoding
	if (!(uMap = globalParams->getTextEncoding()))
	{
		//error(-1, "Couldn't get text encoding");
		delete fileName;
		exitCode = 99;
		return exitCode;
	}

	// open PDF file

	doc = new PDFDoc(fileName, NULL, NULL);

	if (!doc->isOk()) 
	{
		exitCode = 1;
		delete fileName;
		return exitCode;
	}

	// check for copy permission
	if (!doc->okToCopy()) 
	{
		exitCode = 3;
		delete fileName;
		delete doc;
		uMap->decRefCnt();
		return exitCode;
	}

	// get page range
	if (firstPage < 1) 
	{
		firstPage = 1;
	}
	if (lastPage < 1 || lastPage > doc->getNumPages())
	{
		lastPage = doc->getNumPages();
	}

	// write text file
	textOut = new TextOutputDev(out, physLayout, fixedPitch, rawOrder, htmlMeta);
	if (textOut->isOk())
	{
		doc->displayPages(textOut, firstPage, lastPage, 72, 72, 0, gFalse, gTrue, gFalse);
		delete textOut;
		exitCode = 0;
		delete fileName;
		try
		{
			delete doc;
		}
		catch(...)
		{}
		uMap->decRefCnt();
		return exitCode;
	}
	else
	{
		delete textOut;
		exitCode = 2;
		delete fileName;
		try
		{
			delete doc;
		}
		catch(...)
		{}
		uMap->decRefCnt();
		return exitCode;
	}
}
