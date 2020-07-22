# ThreeHandshake
Every connection between nodes should be done in three steps. 
## Step number one:
Exchange blockchain hash and blockchain size. This defines who's the teacher and who's the student. If both hash and block size match, step two will be skipped.
The teacher is the node with fewer blocks in the chain. The student then sends an array of all hashes to the teacher. Both nodes now have to decide wether to trust
the other or not, if the hashes are not equal.
## Step number two:
The teacher will now send all blocks after the end of the students chain to the student. The student then verifies each of the blocks. T´hen it responds true, if all 
blocks are valid. He should now have the same blockchain as the teacher.
## Step number three:
Exchange node list and Lazypool. Both nodes exchange a list of all nodes they know, so nodes can find new nodes and the lazy pool to be up to date.