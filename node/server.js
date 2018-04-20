const dgram = require('dgram')
const util = require('util')
const socket = dgram.createSocket('udp4')

const CONNECT_EVENT_ID = 1
const DISCONNECT_EVENT_ID = 2
const UPDATE_EVENT_ID = 3

const knownClients = {}

function addKnownClient(id, rinfo) {
  knownClients[id] = { rinfo }
}

function removeKnownClient(id) {
  delete knownClients[id]
}

function broadcastUpdateToKnownClients(buffer) {
  Reflect.ownKeys(knownClients)
    .map(clientId => knownClients[clientId])
    .forEach(rinfo => socket.send(buffer, rinfo.port, rinfo.address))
}

socket.on('message', function (message, rinfo) {
  var eventId = "PING"

  switch (message[0]) {
    case CONNECT_EVENT_ID:
      eventId = "CONNECT"
      addKnownClient(message.readUInt32BE(1))
      break
    case DISCONNECT_EVENT_ID:
      eventId = "DISCONNECT"
      removeKnownClient(message.readUInt32BE(1))
      break
    case UPDATE_EVENT_ID:
      eventId = "UPDATE"
      broadcastUpdateToKnownClients(message)
      break
  }

  console.log(`---
Received message:
type: ${eventId}
message: ${util.inspect(message)}
rinfo: ${util.inspect(rinfo)}`)
})

socket.bind(12345, () => console.log('Listening...'))
