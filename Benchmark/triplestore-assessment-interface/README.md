# Hercules triple store assessment interface

Interface relying on the SPARQL endpoint of triplestores, criterion and assessements.

## Build

1. Clone this repo
2. `pip install -r requirements.txt`

## Run

1. Set the server port in [config.json](config.json) (defaults to `5000`)
2. You can change the SPARQL endpoint in [hercules/settings.py](hercules/settings.py)
3. `python server.py`
4. Go to `localhost:{the_port}`
