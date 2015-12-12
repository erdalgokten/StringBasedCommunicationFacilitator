# String-based Communication Facilitator
* Some systems use flat strings to communicate with outer systems.
* String communications are hard and error-prone.
* In this project, T4 template engine has been used to facilitate this communication type.
* T4 templates have been used to create classes from string definitions.
* Then, these classes are defined as WCF classes. By doing that the service can be called SOAP.
* Also, added RESTful capability, therefore RESTful calls are also possible.

# Key points
* The files in TemplateActions and TemplateCustomTypes folders and GeneratedTypes.tt, must be selected as None in their Build properties.
* Also their Copy to output properties must be selected as Do not copy. Otherwise they will be included in the release folder, which does not seem pretty. They are only there for class generations in design-time.
* Desing time class generation can be looked at: https://msdn.microsoft.com/en-us/library/dd820620.aspx

# Includes
* T4 Template Engine
* Extension Methods
* Generics
* Linq
* Xml.Linq
