const dgram = require('dgram')
const util = require('util')
const socket = dgram.createSocket('udp4')

const CONNECT_EVENT_ID = 1
const DISCONNECT_EVENT_ID = 2
const UPDATE_EVENT_ID = 3

const knownClients = {}

function addKnownClient(id, rinfo, joinMessage) {
  knownClients[id] = { rinfo, joinMessage }
}

function removeKnownClient(id) {
  delete knownClients[id]
}

function broadcastUpdateToKnownClients(buffer) {
  Reflect.ownKeys(knownClients)
    .map(clientId => knownClients[clientId].rinfo)
    .forEach(rinfo => socket.send(buffer, rinfo.port, rinfo.address))
}

function broadcastAllJoinsToClient(rinfo) {
  Reflect.ownKeys(knownClients)
    .map(clientId => knownClients[clientId].joinMessage)
    .forEach(joinMessage => socket.send(joinMessage, rinfo.port, rinfo.address))
}

socket.on('message', function (message, rinfo) {
  var eventId = "PING"

  switch (message[0]) {
    case CONNECT_EVENT_ID:
      eventId = "CONNECT"
      broadcastAllJoinsToClient(rinfo)
      addKnownClient(message.readUInt32BE(1), rinfo, message)
      break
    case DISCONNECT_EVENT_ID:
      eventId = "DISCONNECT"
      removeKnownClient(message.readUInt32BE(1))
      break
    case UPDATE_EVENT_ID:
      eventId = "UPDATE"
      break
  }

  broadcastUpdateToKnownClients(message)

  console.log(`---
Received message:
type: ${eventId}
message: ${util.inspect(message)}
rinfo: ${util.inspect(rinfo)}`)
})

socket.bind(12345, () => console.log('Listening...'))
