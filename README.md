# Messenger Vision

## Getting started
Clone the repository and build the solution.

Next, get two or more people together to test: Multiple instances of the same UWP app cannot be run on a single PC.

## Running the server
First, run the server and note down the IP address. The first time the server runs, it creates a database file and a settings file. This file is read on startup and contains some configurables such as the port number, the use of SQLite WAL/Shared cache. It is also possible to force the use of either PLINQ or LINQ.

## Running the client
Next, start some clients and make sure the clients can reach the server (default port number is 5000). The IP address and port number can be changed from the settings menu. It is also possible to choose PLINQ over regular LINQ. The settings cannot be changed once connected.

## Using Messenger-Vision
With a new server, you will first have to register for an account. This can be done by clicking on "Register". Fill in the details and everything should go smoothly. Next, one person has to create a group using "Groups" -> "Add group" from the main window which others can then join using "Groups" -> "Join group". Every group you partake in will be displayed on the left.

To send a message, first select a group from the lefthandside and then type some text. Hit enter or the send button to send a message. Images can also be send using the paperclick icon.

Finally, if you want to make a backup of all your messages, click export on the menubar. There, you can choose a suitable location for the created csv-file.

Sincerly, the creators of Messenger-Vision:
- Jochem Brans
- Tim Gels
- Johannes Kauffmann
- Sietze Koonstra
- Ruben Kuilder
- Rik van Rijn