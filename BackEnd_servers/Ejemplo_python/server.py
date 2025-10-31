from http.server import BaseHTTPRequestHandler, HTTPServer
from datetime import datetime

class Handler(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == '/test':
            self._send(200, 'everything OK')
        elif self.path == '/':
            self._send(200, 'Hello World!')
        elif self.path == '/date':
            self._send(200, datetime.now().astimezone().ctime())
        else:
            self._send(404, 'Not Found')

    def _send(self, status, body):
        self.send_response(status)
        self.send_header('Content-Type', 'text/plain; charset=utf-8')
        self.end_headers()
        self.wfile.write(body.encode('utf-8'))

if __name__ == '__main__':
    server = HTTPServer(('localhost', 3000), Handler)
    print('Server running at http://localhost:3000')
    server.serve_forever()
