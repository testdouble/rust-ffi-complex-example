const dgram = require('dgram')
const util = require('util')
const socket = dgram.createSocket('udp4')

const CONNECT_EVENT_ID = 1
const DISCONNECT_EVENT_ID = 2
const UPDATE_EVENT_ID = 3

socket.on('message', function (message, rinfo) {
  var eventId = "PING"

  switch (message[0]) {
    case CONNECT_EVENT_ID:
      eventId = "CONNECT"
      break
    case DISCONNECT_EVENT_ID:
      eventId = "DISCONNECT"
      break
    case UPDATE_EVENT_ID:
      eventId = "UPDATE"
      break
  }

  console.log(`---
Received message:
type: ${eventId}
message: ${util.inspect(message)}
rinfo: ${util.inspect(rinfo)}`)
})

socket.bind(12345, () => console.log('Listening...'))
