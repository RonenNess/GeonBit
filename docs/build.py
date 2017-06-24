"""
README.md contains the entire readme document, with all chapters.
This script breaks the main readme file into chapters and create a file with links to them.
"""
# read entire file
readme = open("README.md", "r").read()

# break into chapters
chapters = readme.split("\n# ")
print("Found", len(chapters), "potential chapters.")

# will contain a list of chapter names and the urls
out_chapters = []

# iterate chapters
for chapter in chapters:

    # get current chapter header
    header = chapter.split('\n')[0]

    # fix assets path
    chapter = chapter.replace('(assets/', '(../assets/')

    # if special chars appear in chapter name its the opening image, skip it
    if '[' in header:
        continue

    # if chapter header is just 'GeonBit', its still part of the opening, skip it
    if header == 'GeonBit':
        continue

    # create chapter filename
    chapter_filename = "chapters/" + header.lower().replace(' ', '_') + ".md"

    # add to output list
    out_chapters.append((header, chapter_filename))

    # write chapter file
    with open(chapter_filename, 'w') as chapter_file:
        chapter_file.write('# ' + chapter)

# print total chapters
print("\nOutput chapters: ")
for i in out_chapters:
    print(i)

# build table of content
toc = ""
for i in out_chapters:
    toc += "[%s](%s)\n\n" % (i[0], i[1])

# create TOC file
with open("table_of_content.md", 'w') as outfile:
    outfile.write("""![GeonBit](assets/GeonBit-sm.png "GeonBit")

# GeonBit

**A 3D Entity-Component-System engine, powered by MonoGame for C# games.**

## Table Of Contents

__toc__

Or read the whole readme file in one chunk [here](README.md).
""".replace('__toc__', toc))
