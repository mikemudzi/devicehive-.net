﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeviceHive.DocGenerator.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\Templates\PartialResource.cshtml"
    using DeviceHive.DocGenerator;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    public partial class PartialResource : RazorGenerator.Templating.RazorTemplateBase
    {
#line hidden

        #line 4 "..\..\Templates\PartialResource.cshtml"

    public MetadataResource Resource { get; set; }

        #line default
        #line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");


WriteLiteral("\r\n");


WriteLiteral("\r\n\r\n<h1>");


            
            #line 8 "..\..\Templates\PartialResource.cshtml"
Write(Html.Encode(Resource.Name));

            
            #line default
            #line hidden
WriteLiteral("</h1>\r\n<p>");


            
            #line 9 "..\..\Templates\PartialResource.cshtml"
Write(Html.Documentation(Resource.Documentation));

            
            #line default
            #line hidden
WriteLiteral("</p>\r\n\r\n<h2>Methods</h2>\r\n<table>\r\n    <tr>\r\n        <th style=\"width:120px\">Meth" +
"od</th>\r\n        <th style=\"width:150px\">Authorization</th>\r\n        <th style=\"" +
"width:300px\">Uri</th>\r\n        <th style=\"width:400px\">Description</th>\r\n    </t" +
"r>\r\n");


            
            #line 19 "..\..\Templates\PartialResource.cshtml"
     foreach (var method in Resource.Methods)
    {

            
            #line default
            #line hidden
WriteLiteral("    <tr>\r\n        <td><a href=\"#Reference/");


            
            #line 22 "..\..\Templates\PartialResource.cshtml"
                           Write(Html.Encode(Resource.Name));

            
            #line default
            #line hidden
WriteLiteral("/");


            
            #line 22 "..\..\Templates\PartialResource.cshtml"
                                                       Write(Html.Encode(method.Name));

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 22 "..\..\Templates\PartialResource.cshtml"
                                                                                  Write(Html.Encode(method.Name));

            
            #line default
            #line hidden
WriteLiteral("</a></td>\r\n        <td>");


            
            #line 23 "..\..\Templates\PartialResource.cshtml"
       Write(Html.Encode(method.Authorization));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n        <td>");


            
            #line 24 "..\..\Templates\PartialResource.cshtml"
       Write(Html.Encode(method.Verb));

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 24 "..\..\Templates\PartialResource.cshtml"
                                 Write(Html.Encode(method.UriNoQuery()));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n        <td>");


            
            #line 25 "..\..\Templates\PartialResource.cshtml"
       Write(Html.Documentation(method.Documentation));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n    </tr>\r\n");


            
            #line 27 "..\..\Templates\PartialResource.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("</table>\r\n\r\n");


            
            #line 30 "..\..\Templates\PartialResource.cshtml"
 if (Resource.Properties != null)
{

            
            #line default
            #line hidden
WriteLiteral("<h2>Resource Representation</h2>\r\n");


            
            #line 33 "..\..\Templates\PartialResource.cshtml"

            
            #line default
            #line hidden
            
            #line 33 "..\..\Templates\PartialResource.cshtml"
Write(Html.JsonRepresentation(Resource.Properties));

            
            #line default
            #line hidden
            
            #line 33 "..\..\Templates\PartialResource.cshtml"
                                             


            
            #line default
            #line hidden
WriteLiteral("<table>\r\n    <tr>\r\n        <th style=\"width:200px\">Property Name</th>\r\n        <t" +
"h style=\"width:80px\">Type</th>\r\n        <th style=\"width:400px\">Description</th>" +
"\r\n    </tr>\r\n");


            
            #line 41 "..\..\Templates\PartialResource.cshtml"
     foreach (var property in Resource.Properties)
    {

            
            #line default
            #line hidden
WriteLiteral("    <tr>\r\n        <td>");


            
            #line 44 "..\..\Templates\PartialResource.cshtml"
       Write(Html.Encode(property.Name));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n        <td>");


            
            #line 45 "..\..\Templates\PartialResource.cshtml"
       Write(Html.Encode(property.Type));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n        <td>");


            
            #line 46 "..\..\Templates\PartialResource.cshtml"
       Write(Html.Documentation(property.Documentation));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n    </tr>\r\n");


            
            #line 48 "..\..\Templates\PartialResource.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("</table>\r\n");


            
            #line 50 "..\..\Templates\PartialResource.cshtml"
}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591
