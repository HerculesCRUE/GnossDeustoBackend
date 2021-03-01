FROM python:3.7
WORKDIR /bridge
COPY Pipfile* ./
RUN pip install pipenv && pipenv install --system --deploy
COPY . .
CMD ["python3","app.py","0000-0001-8055-6823","--host","0.0.0.0"]
