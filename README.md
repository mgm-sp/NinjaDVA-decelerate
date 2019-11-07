decelerate
==========

## Warning
:bangbang: This application purposely contains vulnerabilities. Use with caution!

## Description
This tool allows the audience to give feedback about the talking speed of a presenter.
Additionally, it serves as a demo application with vulnerabilities.
All information about the vulnerabilities and possible attacks is contained in the [attacks](attacks) directory,
the rest is kept spoiler-free.

## Configuration
Configuration is done in `decelerate/appsettings.json`.
* **Database ⇒ Connection**
    * path to the sqlite database
    * `Data Source=path/to/sqlite.db`
* **JwtKey**
  * key used for signing the JWTs
  * must be **exactly** 32 characters long
* **UserTimeoutSeconds**
  * time after which an inactive user gets logged out automatically (in seconds)
  * after automatic logout, the vote of the user is removed and the username is available again

## Docker Container
To create the docker container, change into the `decelerate` directory and execute
```
docker build -t decelerate .
```

Afterwards, to start it on port `8080`, execute
```
docker run -p 8080:80 decelerate
```

## NinjaDVA Integration
1. Clone the NinjaDVA repository and change into the created directory:
```shell
git clone https://github.com/mgm-sp/NinjaDVA.git
cd NinjaDVA
```
2. Clone the decelerate repository into a directory named `decelerate_vm`:
```shell
git clone git@spcode.mgm-edv.de:NinjaDVA/decelerate.git decelerate_vm
```
3. Configure decelerate (change *JwtKey*):
```shell
vim decelerate_vm/decelerate/appsettings.json
```
4. Start NinjaDVA as usual:
```shell
./ninjadva up
```
5. The application is now available at `decelerate.your-domain.tld`.

To apply changes made to the application or the configuration, you need to trigger provisioning manually after
you started the VM:
```shell
cd decelerate_vm
vagrant provision
```

## User Interface
When you visit the index page, you will see the following:

![Index page](screenshots/homepage.png)

To use the application, you need to enter a name and press *Start*.
This reserves the entered name for you and logs you in.
However, after a certain time of inactivity or after you logged out, the name will be available again.
When you entered a name that isn't already reserved, you will see the following screen:

![User area](screenshots/userarea.png)

To vote, simply click on a position on the slider or drag it, more to the left if you want the talk to be slower
or more to the right if you want it to be faster.
You can logout using the link at the top of the page, but note that this will **remove your vote** and will make
your name available to use by anyone.

## Presenter Interface
The presenter interface is available at `http://your-hostname:port/PresenterArea`. 
After you entered the credentials configured in `decelerate/appsettings.json`, you will see the following page:

![Presenter area](screenshots/presenterarea.png)

The large message box gives advice on your talking speed with one of these messages:
* “talk much slower” (red)
* “talk a bit slower” (yellow)
* “keep your speed” (green)
* “talk a bit faster” (yellow)
* “talk much faster” (red)

Which message is displayed depends on the average vote of all the users, which is also shown using the dot inside
the large red/yellow/green box below.
At the bottom of the page is a table which lists all the users and their individual votes.
Using the links at the top of the page you can reset all votes or delete all users.
The page is updated automatically each time a user logs in, votes or logs out.

## Todo List
* slides
* adapt text for websocket attack to new interface
* add help texts to all new pages
* create new screenshots and document the interface
