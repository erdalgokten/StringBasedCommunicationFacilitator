using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace StringBasedCommunicationFacilitator.Util
{
    public class MetadataFromXml
    {
        public static string PROJ_PATH = string.Empty;

        public List<MessageTemplate> MessageTemplates = new List<MessageTemplate>();
        public List<CustomType> CustomTypes = new List<CustomType>();

        public void LoadActions()
        {
            string messageTemplatesFolder = Path.Combine(PROJ_PATH, "TemplateActions");
            string[] files = Directory.GetFiles(messageTemplatesFolder);
            foreach (string filePath in files)
            {
                LoadMessageTemplateFromFile(filePath);
            }
        }

        public void LoadCustomTypes()
        {
            string customTypesFolder = Path.Combine(PROJ_PATH, "TemplateCustomTypes");
            string[] files = Directory.GetFiles(customTypesFolder);
            foreach (string filePath in files)
            {
                LoadCustomTypeFromFile(filePath);
            }
        }

        private void LoadMessageTemplateFromFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            IEnumerable<XElement> templateEls =
                from el in doc.Root.Elements()
                where el.Name.LocalName == "messageTemplate"
                select el;

            foreach (XElement templateEl in templateEls)
            {
                MessageTemplate template = new MessageTemplate();

                foreach (XAttribute templateAttr in templateEl.Attributes())
                {
                    switch (templateAttr.Name.LocalName)
                    {
                        case "actionName": template.ActionName = templateAttr.Value; break;
                        case "actionDesc": template.ActionDesc = templateAttr.Value; break;
                    }
                }

                IEnumerable<XElement> paramEls =
                    from el in templateEl.Elements()
                    where el.Name.LocalName == "requestHeaderParams"
                    select el;

                foreach (var paramEl in paramEls)
                {
                    foreach (var paramEli in paramEl.Elements())
                    {
                        var param = this.MakeParamObject<RequestHeaderParam>(paramEli);
                        template.RequestHeaderParams.Add(param);
                    }
                }

                paramEls =
                    from el in templateEl.Elements()
                    where el.Name.LocalName == "requestBodyParams"
                    select el;

                foreach (var paramEl in paramEls)
                {
                    foreach (var paramEli in paramEl.Elements())
                    {
                        var param = this.MakeParamObject<RequestBodyParam>(paramEli);
                        template.RequestBodyParams.Add(param);
                    }
                }

                paramEls =
                    from el in templateEl.Elements()
                    where el.Name.LocalName == "responseHeaderParams"
                    select el;

                foreach (var paramEl in paramEls)
                {
                    foreach (var paramEli in paramEl.Elements())
                    {
                        var param = this.MakeParamObject<ResponseHeaderParam>(paramEli);
                        template.ResponseHeaderParams.Add(param);
                    }
                }

                paramEls =
                    from el in templateEl.Elements()
                    where el.Name.LocalName == "responseBodyParams"
                    select el;

                foreach (var paramEl in paramEls)
                {
                    foreach (var paramEli in paramEl.Elements())
                    {
                        var param = this.MakeParamObject<ResponseBodyParam>(paramEli);
                        template.ResponseBodyParams.Add(param);
                    }
                }

                this.MessageTemplates.Add(template);
            }
        }

        private void LoadCustomTypeFromFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            IEnumerable<XElement> templateEls =
                from el in doc.Root.Elements()
                where el.Name.LocalName == "customType"
                select el;

            foreach (XElement customTypeEl in templateEls)
            {
                CustomType template = new CustomType();

                foreach (XAttribute templateAttr in customTypeEl.Attributes())
                {
                    switch (templateAttr.Name.LocalName)
                    {
                        case "name": template.Name = templateAttr.Value; break;
                    }
                }

                IEnumerable<XElement> paramEls =
                    from el in customTypeEl.Elements()
                    where el.Name.LocalName == "fields"
                    select el;

                foreach (var paramEl in paramEls)
                {
                    foreach (var paramEli in paramEl.Elements())
                    {
                        var param = this.MakeParamObject<CustomTypeField>(paramEli);
                        template.CustomTypeFields.Add(param);
                    }
                }

                this.CustomTypes.Add(template);
            }
        }

        private T MakeParamObject<T>(XElement el) where T : FieldDefinition, new()
        {
            T fd = new T();
            foreach (XAttribute attr in el.Attributes())
            {
                switch (attr.Name.LocalName)
                {
                    case "name": fd.Name = attr.Value; break;
                    case "length": fd.Length = int.Parse(attr.Value); break;
                    case "csharpType": fd.CSharpType = attr.Value; break;
                    case "alignment": fd.Alignment = attr.Value; break;
                    case "paddingChar": fd.PaddingChar = attr.Value[0]; break;
                    case "format": fd.Format = attr.Value; break;
                    case "defaultValue": fd.DefaultValue = attr.Value; break;
                    case "isSerializable": fd.IsSerializable = bool.Parse(attr.Value); break;
                    case "ignoreDecimalPoint": fd.IgnoreDecimalPoint = bool.Parse(attr.Value); break;
                }
            }
            return fd;
        }
    }

    /// <summary>
    /// actionName          : Action short name
    /// actionDesc          : Action description
    /// </summary>
    public class MessageTemplate
    {
        public StringBuilder Errors = new StringBuilder();

        private string actionName;
        public string ActionName
        {
            get
            {
                return this.actionName;
            }
            set
            {
                this.actionName = value;
            }
        }

        private string actionDesc;
        public string ActionDesc
        {
            get
            {
                return this.actionDesc;
            }
            set
            {
                this.actionDesc = value;
            }
        }

        public List<RequestHeaderParam> RequestHeaderParams = new List<RequestHeaderParam>();
        public List<RequestBodyParam> RequestBodyParams = new List<RequestBodyParam>();
        public List<ResponseHeaderParam> ResponseHeaderParams = new List<ResponseHeaderParam>();
        public List<ResponseBodyParam> ResponseBodyParams = new List<ResponseBodyParam>();
    }

    public class CustomType
    {
        private string name;
        public string Name
        {
            get
            {
                return this.name.Replace('-', '_');
            }
            set
            {
                this.name = value.Replace('-', '_');
            }
        }

        public List<CustomTypeField> CustomTypeFields = new List<CustomTypeField>();

        public int Length
        {
            get
            {
                int len = 0;
                foreach (CustomTypeField field in this.CustomTypeFields)
                {
                    if (field.IsSerializable)
                    {
                        if (!field.IsCustomType)
                        {
                            len += field.Length;
                        }
                        else
                        {
                            MetadataFromXml metaFromXml = new MetadataFromXml();
                            metaFromXml.LoadCustomTypes();
                            CustomType customType = metaFromXml.CustomTypes.First(c => c.Name == field.Name);
                            len += customType.Length;
                        }
                    }
                }
                return len;
            }
        }
    }

    public class RequestHeaderParam : FieldDefinition
    {
        public RequestHeaderParam() { }
    }

    public class RequestBodyParam : FieldDefinition
    {
        public RequestBodyParam() { }
    }

    public class ResponseHeaderParam : FieldDefinition
    {
        public ResponseHeaderParam() { }
    }

    public class ResponseBodyParam : FieldDefinition
    {
        public ResponseBodyParam() { }
    }
    
    public class CustomTypeField : FieldDefinition
    {
        public CustomTypeField() { }
    }

    /// <summary>
    /// name                : Parameter name, must be compliant with .net field naming standards
    ///   Applies to        : All parameters
    /// length              : Parameter length as string
    ///   Applies to        : Only applies to parameters whose csharpType is string, for other types the length of the format is used
    /// alignment           : Alignment of the string when being sent to backing system. Use this to override the default behavior
    ///   Applies to        : Only applies to parameters whose csharpType is string, DateTime?, TimeSpan?
    /// csharpType          : Underlying C# type. Accepted Types(string, int decimal, bool, DateTime, TimeSpan, DateTime?, TimeSpan?)
    ///   Applies to        : All parameters
    /// paddingChar         : The char to pad the string prior to sending to backing system. Use this to override the default behavior
    ///   Applies to        : Only applies to parameters whosse csharpType is string, DateTime?, TimeSpan?
    ///   Default           : " "
    /// defaultValue        : When there is need to override the parameters supplied by the cliend (WCF client) before sending to backing system. It is only applied for request (RequestHeader, RequestBody) params
    ///   Optional          : For all parameters, when using this make sure that your csharpType is nullable
    /// ignoreDecimalPoint  :
    ///   Applies to        : Only applies to parameters whose csharpType is decimal
    ///   Default           : false
    /// isSerializable      : Flag to determine if parameter should be serialized when sending to backing system or when it should be parsed when deserializing from backing system.
    ///   Optional          : For all parameters
    ///   Default           : true
    /// format              : Format of the string when serializing or deserializing
    ///   Applies to        : bool, int, decimal, DateTime, DateTime?, TimeSpan, TimeSpan?
    ///   Sample            : [boo] = {E;H, Y;N, A;P}
    ///   Sample            : [decimal] = {"00000.00000;-0000.00000"}
    ///   Sample            : [TimeSpan] = {"hhmmssf"} // do not use HH for TimeSpan
    ///   Sample            : [DateTime] = {"yyyyMMddHHmmssf"} // use HH for 24 hour clock
    /// </summary>
    public class FieldDefinition
    {
        public StringBuilder Errors = new StringBuilder();

        public FieldDefinition() { }

        private string name;
        public virtual string Name
        {
            get
            {
                return this.name.Replace('-', '_');
            }
            set
            {
                this.name = value.Replace('-', '_');
            }
        }

        private int length = 0;
        public virtual int Length
        {
            get 
            {
                int len = this.length;
                if (len == 0)
                {
                    switch (this.CSharpType)
                    { 
                        case "bool":
                        case "bool?":
                        case "int":
                        case "int?":
                            len = this.Format.Split(';')[0].Length;
                            break;
                        case "decimal":
                        case "decimal?":
                            int decPointCount = this.IgnoreDecimalPoint ? this.Format.Split(';')[0].Split('.').Length - 1 : 0;
                            len = this.Format.Split(';')[0].Length - decPointCount;
                            break;
                        case "DateTime":
                        case "DateTime?":
                        case "TimeSpan":
                        case "TimeSpan?":
                            len = this.Format.Length;
                            break;
                        default:
                            string error = string.Format(@"Fault attribute: {0}; Message: length must be specified", this.Name);
                            this.Errors.AppendLine(error);
                            break;
                    }
                }
                if (len <= 0)
                {
                    string error = string.Format(@"Fault attribute: {0}; Message: length must be greater than zero", this.Name);
                }
                return len;
            }
            set
            {
                this.length = value;
            }
        }

        private string csharpType;
        public virtual string CSharpType
        {
            get
            {
                return this.csharpType.Replace('-', '_');
            }
            set
            {
                this.csharpType = value.Replace('-', '_');
            }
        }

        private string alignment = string.Empty;
        public virtual string Alignment
        {
            get
            {
                string align = this.alignment;
                if (string.IsNullOrWhiteSpace(align))
                {
                    switch (this.CSharpType)
                    { 
                        case "int":
                        case "int?":
                        case "decimal":
                        case "decimal?":
                        case "DateTime":
                        case "DateTime?":
                        case "TimeSpan":
                        case "TimeSpan?":
                            align = "RIGHT";
                            break;
                        default:
                            align = "LEFT"; // string, bool, bool?, CustomType
                            break;
                    }
                }
                else if (align != "LEFT" && align != "RIGHT")
                {
                    string error = string.Format(@"Fault attribute: {0}; Message: alignment must be of ""LEFT"" or ""RIGHT""", this.Name);
                    this.Errors.AppendLine(error);
                }
                return align;
            }
            set
            {
                this.alignment = value;
            }
        }

        private char paddingChar = '\0';
        public virtual char PaddingChar
        {
            get
            {
                char pad = this.paddingChar;
                if (pad == '\0')
                {
                    switch (this.CSharpType)
                    { 
                        case "int":
                        case "int?":
                        case "decimal":
                        case "decimal?":
                        case "DateTime":
                        case "DateTime?":
                        case "TimeSpan":
                        case "TimeSpan?":
                            pad = '0';
                            break;
                        default:
                            pad = ' '; // string, bool, bool?, CustomType
                            break;
                    }
                }
                return pad;
            }
            set
            {
                this.paddingChar = value;
            }
        }

        private string format = string.Empty;
        public virtual string Format
        {
            get
            {
                string fmt = this.format;
                if (string.IsNullOrWhiteSpace(fmt))
                {
                    switch (this.CSharpType)
                    { 
                        case "bool":
                        case "bool?":
                        case "int":
                        case "int?":
                        case "decimal":
                        case "decimal?":
                        case "DateTime":
                        case "DateTime?":
                        case "TimeSpan":
                        case "TimeSpan?":
                            string error = string.Format(@"Fault attribute: {0}; Message: format must be specified", this.Name);
                            this.Errors.AppendLine(error);
                            break;
                    }
                }

                switch (this.CSharpType)
                { 
                    case "bool":
                    case "bool?":
                        if (fmt.Split(';').Length != 2)
                        {
                            string error = string.Format(@"Fault attribute: {0}; Message: format for bool must have two values separated by semicolon", this.Name);
                            this.Errors.AppendLine(error);
                        }
                        break;
                    case "int":
                    case "int?":
                        if (fmt.Split(';')[0].Contains('.'))
                        {
                            string error = string.Format(@"Fault attribute: {0}; Message: format for int can not have decimal point", this.Name);
                            this.Errors.AppendLine(error);
                        }
                        break;
                    case "decimal":
                    case "decimal?":
                        if (fmt.Split(';')[0].Split('.').Length > 2)
                        {
                            string error = string.Format(@"Fault attribute: {0}; Message: format for decimal can have only one decimal point", this.Name);
                            this.Errors.AppendLine(error);
                        }
                        break;
                    case "DateTime":
                    case "DateTime?":
                        break;
                    case "TimeSpan":
                    case "TimeSpan?":
                        break;
                }
                return fmt;
            }
            set
            {
                this.format = value;
            }
        }

        private string defaultValue;
        public virtual string DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
            set
            {
                this.defaultValue = value;
            }
        }

        private bool isSerializable = true;
        public virtual bool IsSerializable
        {
            get
            {
                return this.isSerializable;
            }
            set
            {
                this.isSerializable = value;
            }
        }

        private bool ignoreDecimalPoint = false;
        public virtual bool IgnoreDecimalPoint
        {
            get
            {
                return this.ignoreDecimalPoint;
            }
            set
            {
                this.ignoreDecimalPoint = value;
            }
        }

        public virtual bool IsCustomType
        {
            get
            {
                switch (this.CSharpType)
                { 
                    case "string":
                    case "bool":
                    case "bool?":
                    case "int":
                    case "int?":
                    case "decimal":
                    case "decimal?":
                    case "DateTime":
                    case "DateTime?":
                    case "TimeSpan":
                    case "TimeSpan?":
                        return false;
                    default:
                        return true;
                }
            }
        }
    }
}