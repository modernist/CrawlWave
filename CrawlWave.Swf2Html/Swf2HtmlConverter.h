// Swf2HtmlConverter.h : Declaration of the CSwf2HtmlConverter

#pragma once
#include "resource.h"       // main symbols


// ISwf2HtmlConverter
[
	object,
	uuid("5859F8E0-3A39-401E-8220-D1AE6542EEC1"),
	dual,	helpstring("ISwf2HtmlConverter Interface"),
	pointer_default(unique)
]
__interface ISwf2HtmlConverter : IDispatch
{
	[id(1), helpstring("Converts a SWF File to HTML")] HRESULT ConvertSwfFile([in] BSTR inputFile, [in] BSTR outputFile, [out,retval] VARIANT_BOOL* success);
};



// CSwf2HtmlConverter

[
	coclass,
	threading("apartment"),
	vi_progid("CrawlWaveSwf2Html.Swf2HtmlConverter"),
	progid("CrawlWaveSwf2Html.Swf2HtmlConverter.1"),
	version(1.0),
	uuid("9E265241-BF33-4C0B-BE93-083914202BBE"),
	helpstring("Swf2HtmlConverter Class")
]
class ATL_NO_VTABLE CSwf2HtmlConverter : 
	public ISwf2HtmlConverter
{
public:
	CSwf2HtmlConverter()
	{
	}


	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}
	
	void FinalRelease() 
	{
	}

public:

	STDMETHOD(ConvertSwfFile)(BSTR inputFile, BSTR outputFile, VARIANT_BOOL* success);
};

