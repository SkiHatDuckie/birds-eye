from setuptools import setup, find_packages

from codecs import open
from os import path

# The directory containing this file
HERE = path.abspath(path.dirname(__file__))

# Get the long description from the README file
with open(path.join(HERE, 'README.md'), encoding='utf-8') as f:
    long_description = f.read()

setup(
    name="birds-eye-lib",
    version="0.3.0",
    description="A BizHawk external tool + python library for python scripts to communicate with the BizHawk emulator.",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://birds-eye.readthedocs.io/",
    author="SkiHatDuckie",
    author_email="SkiHatDuckie@gmail.com",
    license="MIT",
    classifiers=[
        "Intended Audience :: Developers",
        "License :: OSI Approved :: MIT License",
        "Programming Language :: Python",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.6",
        "Programming Language :: Python :: 3.7",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
        "Operating System :: OS Independent"
    ],
    packages=["birdseye"],
    include_package_data=True,
)