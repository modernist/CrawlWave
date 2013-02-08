// Pdf2TextConverter.h : Declaration of the CPdf2TextConverter

#pragma once
#include "resource.h"       // main symbols
#include "aconf.h"
#include <stdio.h>
#include <stdlib.h>
#include <stddef.h>
#include <string.h>
#include "parseargs.h"
#include "GString.h"
#include "gmem.h"
#include "GlobalParams.h"
#include "Object.h"
#include "Stream.h"
#include "Array.h"
#include "Dict.h"
#include "XRef.h"
#include "Catalog.h"
#include "Page.h"
#include "PDFDoc.h"
#include "TextOutputDev.h"
#include "CharTypes.h"
#include "UnicodeMap.h"
#include "Error.h"
#include "config.h"

// IPdf2TextConverter
[
	object,
	uuid("71603162-6E69-4EF0-8217-61DC05158375"),
	dual,	helpstring("IPdf2TextConverter Interface"),
	pointer_default(unique)
]
__interface IPdf2TextConverter : IDispatch
{
	[id(1), helpstring("Converts a PDF file to Text")] HRESULT ConvertPdf2Text([in] BSTR inputFile, [in] BSTR outputFile, [out,retval] LONG* success);
};



// CPdf2TextConverter

[
	coclass,
	threading("apartment"),
	vi_progid("CrawlWavePdf2Text.Pdf2TextConverter"),
	progid("CrawlWavePdf2Text.Pdf2TextConverter.1"),
	version(1.0),
	uuid("47BC81E2-CAA1-4D32-BAA5-E8DC61A65D52"),
	helpstring("Pdf2TextConverter Class")
]
class ATL_NO_VTABLE CPdf2TextConverter : 
	public IPdf2TextConverter
{
private:
	bool isInitialized;
	int firstPage;
	int lastPage;
	GBool physLayout;
	GBool rawOrder;
	GBool htmlMeta;
	double fixedPitch;
	char textEncName[128];
	char textEOL[16];
	GBool noPageBreaks;
	char ownerPassword[33];
	char userPassword[33];
	GBool quiet;
	char cfgFileName[8];
	GBool printVersion;
	GBool printHelp;

	int PerformConversion(char *in, char *out);

public:
	CPdf2TextConverter()
	{
		firstPage = 1;
		lastPage = 0;
		physLayout = gFalse;
		rawOrder = gFalse;
		htmlMeta = gFalse;
		fixedPitch = 0;
		strcpy(textEncName, "ISO-8859-7");
		strcpy(textEOL, "");
		noPageBreaks = gFalse;
		strcpy(ownerPassword, "\001");
		strcpy(userPassword, "\001");
		quiet = gTrue;
		strcpy(cfgFileName, "xpdfrc");
		printVersion = gFalse;
		printHelp = gFalse;
		// read config file
		globalParams = new GlobalParams(cfgFileName);
		globalParams->setTextEncoding(textEncName);

		if (quiet)
		{
			globalParams->setErrQuiet(quiet);
		}
	}

	~CPdf2TextConverter()
	{
		if(globalParams)
		{
			try
			{
				delete globalParams;
			}
			catch(...)
			{}
		}
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

	STDMETHOD(ConvertPdf2Text)(BSTR inputFile, BSTR outputFile, LONG* success);
};

