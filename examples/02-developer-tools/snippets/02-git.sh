# === A complete Git and GitHub workflow ===
dotnet new console -o MyTool && cd MyTool
git init -b main
dotnet new gitignore              # ignore bin/, obj/ and editor files
git add .
git commit -m "Create console project"

# Work on a branch, never directly on main.
git switch -c feature/add-greeting
#   ...edit Program.cs...
git status                        # what changed?
git diff                          # exactly which lines?
git add Program.cs
git commit -m "Add greeting with user name"

# Publish and open a pull request on GitHub.
git remote add origin https://github.com/<you>/MyTool.git
git push -u origin feature/add-greeting

# After the pull request is merged:
git switch main
git pull
git branch -d feature/add-greeting

# Looking around and undoing
git log --oneline --graph --all   # history as a graph
git restore Program.cs            # throw away uncommitted changes to a file
git revert <commit>               # new commit that undoes an old one (safe on shared branches)
