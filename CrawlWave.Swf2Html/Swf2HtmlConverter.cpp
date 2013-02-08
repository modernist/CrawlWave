// Swf2HtmlConverter.cpp : Implementation of CSwf2HtmlConverter

#include "stdafx.h"
#include "Swf2HtmlConverter.h"
#include ".\swf2htmlconverter.h"
#include "swf2html_stdio.h"

// CSwf2HtmlConverter


STDMETHODIMP CSwf2HtmlConverter::ConvertSwfFile(BSTR inputFile, BSTR outputFile, VARIANT_BOOL* success)
{
	USES_CONVERSION;

	VARIANT_BOOL retVal = true;
	char *input, *output;

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

	Swf2HtmlConverterStdio *converter = new Swf2HtmlConverterStdio();
	try
	{
		retVal= converter->ConvertSwf2Html(input, output);
	}
	catch(...)
	{
		retVal = false;
	}
	*success = retVal;
	delete converter;
	return S_OK;
}
