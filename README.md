decelerate
==========

## Warning
:bangbang: This application purposely contains vulnerabilities. Use with caution!

## Description
This tool allows the audience to give feedback about the talking speed of a presenter.
Additionally, it serves as a demo application with vulnerabilities.

## Vulnerabilities
* JSON Web Tokens with signing algorithm `none` get accepted
* JWT Payload contains type information which is not validated before deserializing it
* WebSocket for presenter interface is accessible from arbitrary origins

## Configuration
Configuration is done in `decelerate/appsettings.json`.
* **Database ⇒ Connection**
    * path to the sqlite database
    * `Data Source=path/to/sqlite.db`
* **JwtKey**
  * key used for signing the JWTs
  * not a huge secret because unsigned JWTs are accepted anyway (as long as the algorithm is set to `none`,
    otherwise the signature is checked)
  * must be **exactly** 32 characters long
* **UserTimeoutSeconds**
  * time after which an inactive user gets logged out automatically (in seconds)
  * after automatic logout, the vote of the user is removed and the username is available again
* **Presenter ⇒ Username/Password**
  * username and password for the presenter area
  * also not a huge secret because a lot of the available information is also accessible via the WebSocket
    (which requires authentication but is not protected from cross-site requests)

## Todo List
* implement time decay for the votes?
* add function to clear votes / users / ...? (after this is added the presenter credentials become more important!)
