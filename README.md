# Innovator Token Connection

This is an implementation of Aras IOM IServerConnection, allowing you to connect to Innovator using already ussied token, instead of database name, username and password.
Using this class you can connect custom APIs using already established user Innovator session.

## Usage
### Custom API configuration
Use TokenHttpConnection instance to create Innovator class:
Innovator ret = IomFactory.CreateInnovator(new TokenHttpConnection(token, url));

### Innovator configuration
TokenHttpConnection will try to invoke get_innovator_session_info server method in order to retrieve the user session information. Te returned XML must contains the following elements: User (with attribute id), db_name and license_info.

Here is an example code of get_innovator_session_info:
Innovator inn = this.getInnovator();
var info = inn.newItem("User", "get");
info.setProperty("id", inn.getUserID());
info = info.apply();
info.setProperty("db_name", inn.getConnection().GetDatabaseName());
info.setProperty("license_info", inn.getConnection().GetLicenseInfo());
return info;



The token must be passes via "token" url parameter. It can be retrieved in server method using HttpContext.Current.Request.Headers["Authorization"]
Example:
http://my_api_url/?token=12337y24987374ab66....
