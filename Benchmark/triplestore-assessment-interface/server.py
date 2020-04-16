# -*- coding: utf-8 -*-
"""Standalone application server."""

import json
with open('config.json') as f:
    cfg = json.load(f)

# Create Hercules app
from hercules.app import create_app
app = create_app()


# If we're running in stand alone mode, run the application
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=cfg['server'].get('port', 5000), debug=True)