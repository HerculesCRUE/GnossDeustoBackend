FROM python:3.6.6
RUN mkdir /benchmark

WORKDIR /benchmark

COPY . /benchmark

RUN pip install -r requirements.txt

RUN python3.6.6 setup.py install

CMD [ "python3.6.6", "server.py" ]
