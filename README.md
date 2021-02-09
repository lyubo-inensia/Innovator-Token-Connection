# Innovator Token Connection

This is an implementation of Aras IOM IServerConnection, allowing you to connect to Innovator using already ussied token, instead of database name, username and password.
Using this class you can connect custom APIs using already established user Innovator session.

## Usage
### Custom API configuration
Use TokenHttpConnection instance to create Innovator class:
```cs
// The token is retrieved from the url with wich the has been called eg. http://my_api_url/?token=12337y24987374ab66....
// url argument is the Aras Innovator url without any paths eg.: /client, /server, oauth etc.
Innovator ret = IomFactory.CreateInnovator(new TokenHttpConnection(token, url));
```
### Innovator configuration
TokenHttpConnection will try to invoke get_innovator_session_info server method in order to retrieve the user session information. Te returned XML must contains the following elements: User (with attribute id), db_name and license_info.
<br/>
Here is an example code of get_innovator_session_info:<br/>
```cs
Innovator inn = this.getInnovator();<br/>
var info = inn.newItem("User", "get");<br/>
info.setProperty("id", inn.getUserID());<br/>
info = info.apply();<br/>
info.setProperty("db_name", inn.getConnection().GetDatabaseName());<br/>
info.setProperty("license_info", inn.getConnection().GetLicenseInfo());<br/>
return info;<br/>
```
<br/>
The token could be passes via url parameter. It can be retrieved in every server method using ```cs HttpContext.Current.Request.Headers["Authorization"]; ```
Example:<br/>
http://my_api_url/?token=12337y24987374ab66....
