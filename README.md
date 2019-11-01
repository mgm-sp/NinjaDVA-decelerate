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
* Websocket for presenter interface is accessible from arbitrary origins

## Todo List
* implement time decay for the votes?
