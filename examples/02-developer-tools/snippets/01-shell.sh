# === Shell basics (bash) ===
pwd                         # where am I
ls -la                      # list files, including hidden ones
cd ~/projects && mkdir demo && cd demo

echo "hello" > notes.txt    # > overwrites a file
echo "world" >> notes.txt   # >> appends
cat notes.txt

# A pipe sends the output of one command into the next.
ls -la | wc -l                              # count directory entries
grep -r "TODO" --include="*.cs" . | wc -l   # count TODO comments in C# files
history | tail -n 20                        # the last 20 commands

# Variables and a loop
name="report"
for i in 1 2 3; do
  touch "${name}_${i}.txt"
done

export DOTNET_ENVIRONMENT=Development   # environment variable for child processes
echo "$DOTNET_ENVIRONMENT"
