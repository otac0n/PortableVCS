Portable VCS Format
===================

The Portable VCS Format is a specification and reference implementation for a
version control history transfer format.

Goal
----

The goal of the Portable VCS Format is to provide a consistent format that
facillitates migration and continued one-way replication between the various
version control systems.

Why is this necessary?
----------------------

I would like to borrow the scenario from Eric Lippert's blog post "[Why IL?](https://blogs.msdn.com/b/ericlippert/archive/2011/11/18/why-il.aspx)"
to illustrate.

> Suppose you have n languages: C#, VB, F#, JScript .NET, and so on. Suppose you
> have m different runtime environments: Windows machines running on x86 or x64,
> XBOX 360, phones, Silverlight running on the Mac... and suppose you go with
> the one-compiler strategy for each. How many compiler back-end code generators
> do you end up writing? For each language you need a code generator for each
> target environment, so you end up writing n x m code generators.
>
> Suppose instead you have every language generate code into IL, and then you
> have one jitter per target environment. How many code generators do you end up
> writing?  One per language to go to IL, and one per environment to go from IL
> to the target machine code. That's only n + m, which is far less than n x m
> for reasonably-sized values of n and m.
>
> ...
>
> The cost savings go the other way too; if you want to support a new chipset
> then you just write yourself a jitter for that chipset and all the languages
> that compile to IL suddenly start working; you only had to write *one* jitter
> to get n languages on your new platform.

This scenario is analagous to the situation today with the landscape of version
control systems.  Today, each system has a certain level of support for
converting to and from various other version control systems: Git has git-svn,
Mercurial has hg-git, and etc.  However, this is poor support for converting
from Mercurial into Git, or from Veracity to CVS.

The Portable VCS Format aims to be the "glue" that allows for free conversion
between all version control systems.

Guidlines for Contributing
--------------------------

The design of the Portable VCS Format is heavily influenced by existing systems.
Any VCS that has a stake in the format has the possibility of influencing the
design.  To remain pragmatic, these guidelines are provided for anyone
interested in contributing to the project:

 * Contributors are encouraged to expand the specification to include support
   for *existing* features of *existing* VCS storage formats.

 * Contributors are encouraged to branch the the specification to include
   support for *upcoming* features of *existing* VCS storage formats.

 * Contributors are discouraged from include purely theoretical features.

In general, if a feature has not been released by a version control system, it
is considered theoretical.  If a new feature shows up in a branch or fork of
a VCS, the corresponding support for it should show up in a branch of Portable
VCS.  When the aformentioned branch or fork is merged into the VCS or released
in its own right, then the corresponding branch should be merged in.
