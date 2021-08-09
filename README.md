# Booking and Chat Web Application

Flight booking and car rental web application based on microservice arhitecture. The application also provides the ability for users to chat with friends.
Users can send messages to their friends, see conversation history, delete conversations, etc.

## Prerequisites

[Docker for Windows](https://docs.docker.com/docker-for-windows/install/)

[Docker for Mac](https://docs.docker.com/docker-for-mac/install/)

[Node.js](https://nodejs.org/en/download/)

Angular cli installation
`npm install -g @angular/cli`

### How to start application?

1. Open command prompt and navigate to new folder.
2. In command prompt type: `git clone https://github.com/bojanpisic/WebApp-Microservice-Arhitecture.git`
3. Navigate command prompt to Web2-Microservices folder and type command `docker-compose up -d` to create and start docker containers
4. Navigate command prompt to Web-SPA folder and type command `npm install` to install node-modules
5. When the installation of modules is complete, type command `ng serve` to start angular application
