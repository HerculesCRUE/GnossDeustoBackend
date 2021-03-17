FROM python:3.7
RUN mkdir /benchmark

WORKDIR /benchmark

COPY . /benchmark

RUN python setup.py install

CMD [ "python", "./server.py" ]
