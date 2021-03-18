FROM python:3.6
RUN mkdir /benchmark

WORKDIR /benchmark

COPY . /benchmark

RUN python3.6 setup.py install

CMD [ "python3.6", "server.py" ]
