# Walrus

A multi-repo Git query tool

## Purpose

I spend a lot of time tracking down what and when I did on a certain day. Usually this spans many (10+) repositories which means the task is boring and tedious. This 
tool aims to automate the discovery of git commits based on date, author, and other criteria. 

## Goals

We don't want to reinvent libgit2sharp, we just want a nice query interface specific to commit messages. We don't care about diffs, code content, or even the changesets.
For this reason, Gitbase was found to be entirely overkill. It is a fantastic tool and I do make use of it but managing it is kind of a pain. For this reason, the 
overarching goal of this project is simplicity with the ability to opt-in to complexity when needed.

## Getting Started

This utility can be compiled and placed on your path. See `tools/publish.ps1` for how to build a minimal binary.

Alternatively (and recommended) you can use `dotnet tool` to create and install a global tool. See `tools/install_as_tool.ps1` for how to do this.

You need a JSON config file to get started located at env WALRUS_CONFIG_FILE or in the same directory as the binary named `walrus.json`. 
The contents should look something like this is:

```
{
  "DirectoryScanDepth": 3,
  "RepositoryRoots": [
    "H:\\code",
  ]
}
```

If you like to keep a flat code directory then you can set `DirectoryScanDepth` to a smaller value.


## Examples

These example assume you have install Walrus.CLI as a `dotnet tool`.

Show the active configuration
```
walrusc show config
```
  
Print a list of all repositories
```
walrusc show repos
```
  
Count commits between March 2 and Jun 2 of 2021. This includes all authors on the currently checked out branch.
```
walrusc query --after "Mar 2 2021" --before "Jun 2 2021"
```
  

Count commits on all branches since last week (default if --after is not specified) and restrict to a single author.
```
walrusc query --all-branches --author-email cory@email.com
```

Print commit summary by repo and date to the console.
```
walrusc query --author-email cory@email.com --print-table

Repository: uarch_benchmarks [file://H:\code\system\uarch_benchmarks]
----------------------------------------------------------------------------------------------------
8/15/2021: 3 Commits
        19:16 2bbde046b8232f58357bf89b1b4478f3275e7367 add cmake build scripts
        19:38 2e630b5e20d2e66e3809641fc648ec3a7eeef3f6 [cmake] use dots in app names
        19:39 bc2232a9bea51d3343eba5e6c818692db7339362 msvc compatibility fix


Repository: Walrus [file://H:\code\tools\Walrus]
----------------------------------------------------------------------------------------------------
8/21/2021: 19 Commits
        12:02 ba29639d9eef1e33ae74d4f487eaabf04ce66afc First commit
        14:01 89421e0332f1db429a150ffac221a2d46454947a add logging to core library
        14:02 31a2cf014c844d0c9da6b93c097e118c95b24efe add ensure helper class
        14:02 0335f9a11ec0c5f83c0f412ed49e816f815d9089 enable nullable on core library
        14:04 f91dab40a622e4617e72619f56954f0896332ff0 fix crash commit enumerator
        14:07 77cfa7fa5bb717902d999e4ce16be8ad7a739379 code cleanup
        14:07 515fec7ad07d728044271e7fa6a93a34217613b3 connect logging to core lib in CLI
        14:08 666dd3aeb13fccd82676bee06e2bd50b5b367265 add query command
        14:10 b59ccd09df2811132752094bfa6307b6fbae3e40 stop tracking config files
        14:10 f47fb2990a0d9596e908a23a25e248b0b10a1f37 ignore test and launch configs
        14:10 a238cd6a745727f527ac0e7f7da55d955c26b437 remove noisy log line
        14:26 79681875e6493fcbfc0496822d440f46ad1a857a add author to query parameters
        14:29 48d258f0a443b68bd8619b982b26db6571de7853 exclude remote branches
        14:33 ce9c3cc6ebbf615c5ee354b28a06d935f0bff8a8 remove walrus service
        15:12 9142eef111af4786c0070fe7f1c75a82564fd861 make query parallel
        15:13 ef7118ce0300dd0455983ff93af22d8cb2413a97 add more info to WalrusCommit
        21:05 4994950950a5fbd27aa722a1568702a586492db7 add repo name filter
        22:24 553bb2cf992eaaf1ae8ee50f1578415715354e65 fix duplicate repo return
        22:36 f2e18d204cfb91b0fdc5dd9c880e676634d68db3 [cli] add table format printer
8/22/2021: 1 Commits
        09:12 82eb14b37e57a665e7077fb4754a8b3e6a8af6de add file link to cli table output


====================================================================================================
Total Commits: 23
====================================================================================================
```

## Roadmap

- [x] Basic date/author query interface 
- [x] Simple CLI 
- [x] Table style CLI output
- [x] Create dotnet tool
- [ ] Calendar style GUI 
