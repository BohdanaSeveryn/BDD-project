"""
Setup configuration for BDD Educational Project.

This allows the project to be installed as a package, making imports cleaner.
For development, install with: pip install -e .
"""

from setuptools import setup, find_packages

setup(
    name="bdd-calculator",
    version="1.0.0",
    description="Educational project demonstrating Behavior-Driven Development with Python and Behave",
    author="BDD Educational Project",
    packages=find_packages(),
    python_requires='>=3.7',
    install_requires=[
        'behave==1.2.6',
        'parse==1.20.0',
        'parse-type==0.6.2',
    ],
    classifiers=[
        'Development Status :: 4 - Beta',
        'Intended Audience :: Education',
        'Topic :: Software Development :: Testing',
        'Programming Language :: Python :: 3',
        'Programming Language :: Python :: 3.7',
        'Programming Language :: Python :: 3.8',
        'Programming Language :: Python :: 3.9',
        'Programming Language :: Python :: 3.10',
        'Programming Language :: Python :: 3.11',
        'Programming Language :: Python :: 3.12',
    ],
)
