# LeagueS4 API Server

### Description

* LeagueS4 API Server - API Server for LeagueS4 Launcher

### Documentation

There is no application documentation. However, there is a REST documentation for developers below.

### Requirements

A MariaDB server is being required.

For the default local profile there is a development database configured in application-dev.properties.

You cannot run the application without a profile (--spring.profiles.active).

In order to modify code in your IDE, please make sure to enable "annotation processing".

In order to run the application in your IDE please set the VM option -DideMode=true. This will ensure that the
application-dev.properties will not be copied and overwritten.

### Instructions for Live Reload

Verify that the option check-box File->Setting –> Build, Execution, Deployment –> Compiler–>Build project automatically
is selected.

Press SHIFT+CTRL+A for Linux/Windows users or Command+SHIFT+A for Mac users, then type registry in the opened pop-up
window. Scroll down to Registry... using the down arrow key and hit ENTER on Registry.... In the Registry window verify
the following option: compiler.automake.allow.when.app.running = true.

### Environment Variables (Only production)

* DATABASE_CONNECTION: Full string for database connection
* DATABASE_USERNAME: Username for the sql database
* DATABASE_PASSWORD: Password for the sql database
* JWT_SECRET: JWT secret
* SERVICE_DOMAIN: devapi/api, version to deploy via CI/CD
* USER_MAX_FRIENDS: Maximum friends a user can have
* USER_MAX_FRIEND_REQUESTS_IN: Maximum friend requests a user can receive
* USER_MAX_FRIEND_REQUESTS_OUT: Maximum friend requests a user can send
* USER_MAX_LOBBY_INVITES: Maximum lobby invites to a user
* MAIL_HOST: Mail server host
* MAIL_PORT: Mail server port
* MAIL_USERNAME: Mail server username
* MAIL_PASSWORD: Mail server password
* ALLOWED_ORIGINS: Origins which are allowed to request the API, comma separated and prefixed with http/https. Set to *
  if all should be allowed.

### Execution Variables

* --spring.profiles.active=production
* --spring.profiles.active=dev

### Used Profiles

1. Default - Never use on a production system!
2. Production - Changes authentication, variables in properties and possible some other classes. Don't use locally!

### Configuration

See *application-dev.properties* and *application-production.properties*

### REST Documentation

You can find the REST documentation at <http://localhost/swagger-ui.html>.

The REST documentation is not available with the production profile.

The Error codes may vary due to internal spring error checks.

### Websocket Documentation

We're using Websockets with the STOMP Sub-Protocol.

#### Authentication

The protocol we use does not support authentication headers. Authentication can be made sending a valid JWT token using
the request parameter 'token'.

#### Stomp Endpoint

/socket?token=XXX

#### Destinations

Incoming: /out/system, /out/lobby, /out/champselect

Outgoing (Subscribable): /user/queue/system, /user/queue/lobby{LOBBY_ID}, /user/queue/champselect{CHAMPSELECT_ID}

#### Data and Data-Flow

See *SocketMessage.java*, *ErrorMessage.java* and *MessageType.java*.

#### Data-Flow

Incoming Request OK:  
Incoming SocketMessage -> Processing -> SocketMessage(null, <Original Message ID>, null, MessageType.java, Timestamp)

Incoming Request OK with Data:  
Incoming SocketMessage -> Processing -> SocketMessage(data, <Original Message ID>, null, MessageType.java, Timestamp)

Incoming Request Error:  
Incoming SocketMessage -> Processing -> SocketMessage(null, <Original Message ID>, ErrorMessage.java, MessageType.java,
Timestamp)

Outgoing:  
*No* Incoming Socket Message -> SocketMessage(data, null, null, MessageType.java, Timestamp)

### HTML-Endpoints

* Coming soon

### Dependencies

* See *pom.xml*
* Lombok (plugin)

### Testing (in IDE)

* Git clone the project
* Install Java 17 and IntelliJ
* Get all the plugins for IntelliJ as mentioned in Code Guidelines in Contributing.md
* Enable annotation processing in IDE settings
* Configure the dev config
* Run the application with VM argument -Dspring.profiles.active=dev
* During the first startup spring will create the database structure. Fill the database with the .sql scripts in the
  sqlinserts folder

### Testing (without IDE)

* Download the newest API jar
* Install Java 17
* Run the application with VM argument -Dspring.profiles.active=dev
* The config file will be created in the same directory as the jar file. Configure it and restart.
* During the first startup spring will create the database structure. Fill the database with the .sql scripts in the
  sqlinserts folder

### Building

* You can build the project with `mvn package -DskipTests -B -f pom.xml`