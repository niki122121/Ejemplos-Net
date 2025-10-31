const http = require('http');

const server = http.createServer((req, res) => {

  res.setHeader('Content-Type', 'text/plain');

  switch (req.url) {
    case '/test':
      return res.end('everything OK');
    case '/':
      return res.end('Hello World!');
    case '/date':
      return res.end(new Date().toString());
    default:
      res.statusCode = 404;
      return res.end('Not Found');
  }
});

server.listen(3000, () => {
  console.log('Server running at http://localhost:3000');
});
