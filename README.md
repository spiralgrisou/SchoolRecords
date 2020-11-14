# School Records system
Features:
* File/Directory database (Can be used in a server but is not great at all for commercial use)
* A Manager's Application for editing the school's database
	* Add or remove student accounts from the database
	* Check the pending meeting requests from students and delete them (A pretty weak way of responding I know...)
* A Student's Application that is essentially a client that reads the database and cannot edit anything in it
	* Has to login with an account that exists in the database to do anything (Anything except editing the console editing the console of course.)
	* Can get to see his own account information obviously.
	* Can request a meeting and then wait for a response from a teacher.
- - - -
<details>
	<summary>Features i was planning to add but i didn't :grin:</summary>
	<p>
		Make a TCP Server for the students to interact with as this is not secured at all, They can get into the database files and get a password of any account.
	</p>
</details>