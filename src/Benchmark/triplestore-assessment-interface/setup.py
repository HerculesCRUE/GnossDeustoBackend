#!/usr/bin/env python

from setuptools import setup

# So you can use either pip or this setup script
with open('requirements.txt') as f:
    requirements = f.read().splitlines()

setup(name='Hercules',
      version='0.1',
      description='Triple Store Benchmark',
      author='Mathieu d\'Aquin',
      author_email='mathieu.daquin@insight-centre.org',
      url='',
      install_requires=requirements,
)
