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

Suppose you have a two programming languages, Quux and Baz, and you would like
both of them to target three different CPU architectures: x86, x64 and ia64.

One possible way to do this would be to build one code generator per combination
of language and CPU architecture.  If you were to go this route, you would end
up with six code generators in total: Quxu-to-x86, Baz-to-x86, Quux-to-x64, and
so on.  If you wanted to create a new language, Farkle, you would have to add
three more code generators.  In general you would have n * m code generators,
where n and m are the number of languages and target platforms, respectively.

Another possible way, would be to compile each language into some sort of shared
"intermediary" language which would then, in turn, be compiled into the machine
code for the specific architecture desired.  In this case, you would need n + m
code generators, n to go from the languages into the shared language, and m to
go into the byte code.

This is the model followed by the Java VM languages (Java Bytecode), the .NET
languages (IL), and even older languages like VB6.  The key to this efficiency
is that the intermediary language must be *shared* between the other languages.
It is not enough, for example, for C# to have its own internal "intermediary"
language, because no other language would be unable to utillize the machine code
generators that C# had.

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
