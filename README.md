# Walrus

A multi-repo Git query tool

## Purpose

I spend a lot of time tracking down what and when I did on a certain day. Usually this spans many (10+) repositories which means the task is boring and tedious. This 
tool aims to automate the discovery of git commits based on date, author, and other criteria. 

## Goals

We don't want to reinvent libgit2sharp, we just want a nice query interface specific to commit messages. We don't care about diffs, code content, or even the changesets.
For this reason, Gitbase was found to be entirely overkill. It is a fantastic tool and I do make use of it but managing it is kind of a pain. For this reason, the 
overarching goal of this project is simplicity with the ability to opt-in to complexity when needed.

## Roadmap

- [x] Basic date/author query interface 
- [x] Simple CLI 
- [ ] Calendar style GUI 
