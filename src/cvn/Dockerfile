FROM python:3.7
WORKDIR /cvn
COPY Pipfile* ./
RUN pip install pipenv && pipenv install --system --deploy
COPY . .
CMD ["python3","-m","cvn.webserver","-p","5104","--host","0.0.0.0"]
