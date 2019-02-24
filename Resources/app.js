var http = require('http')
var url = require('url')
var hash = require('object-hash')

const hostname = '127.0.0.1';
const port = 3001;

const server = http.createServer(function (request, response) {
  let url_parts = url.parse(request.url, true)

  // Chrome requests the favicon. Rather than crash, just return nothing.
  if(url_parts.query === undefined || url_parts.query.obj_to_hash === undefined) {
    return
  }

  let jString = url_parts.query.obj_to_hash
  if (jString.length > 256) {
    return
  }

  let o = ""
  try {
    o = JSON.parse(jString)
  } catch(e) {
    console.warn('Provided obj_to_hash is not valid JSON; defaulting to using it as a string')
    o = jString
  }

  // TODO: accept options from parameters
  response.end(hash(o, {respectType: true, unorderedArrays: true, unorderedSets: true}))
});

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});
