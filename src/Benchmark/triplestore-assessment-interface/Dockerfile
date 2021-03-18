FROM python:3.6.6

RUN mkdir /benchmark

WORKDIR /benchmark

COPY . /benchmark

RUN pip install -r requirements.txt

RUN python setup.py install

CMD [ "python", "server.py" ]
