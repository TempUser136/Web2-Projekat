import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import User from "../Models/User";

function SignUp() {
  const navigate = useNavigate();
  const [token, setToken] = useState('');

  const handleLogin = async (username, password) => {
    try {
      const response = await axios.post('https://localhost:7280/api/users/login', { username, password });
      setToken(response.data.token);
      localStorage.setItem('token', token); // Store the token in localStorage

      console.log('Token:', token);
      // Store token in local storage or context if needed
    } catch (error) {
      console.error('There was an error logging in!', error);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);

    const user = new User(
      formData.get('Username'),
      formData.get('Email'),
      formData.get('Password'),
      formData.get('Name'),
      formData.get('LastName'),
      formData.get('Birthday'),
      formData.get('Address'),
      formData.get('Type'),
      formData.get('Image')
    );

    try {
      const response = await axios.post('http://localhost:8613/user/creteUser',formData,1);
      console.log('User added:', response.data);
      navigate('/');
    } catch (error) {
      console.error('There was an error adding the user!', error);
    }
  };

  return (
    <div className="SignUpBg">
      <label>Sign Up</label>
      <form className="forma" onSubmit={handleSubmit}>
        <input type="text" name="Username" placeholder="Username" /><br />
        <input type="email" name="Email" placeholder="E-mail" /><br />
        <input type="password" name="Password" placeholder="Password" /><br />
        <input type="text" name="Name" placeholder="Name" /><br />
        <input type="text" name="LastName" placeholder="Last name" /><br />
        <label>Birthday:</label>
        <input type="date" name="Birthday" /><br />
        <input type="text" name="Address" placeholder="Address" /><br />
        <label>Type:</label>
        <select name="Type" id="users">
          <option value="Administrator">Administrator</option>
          <option value="User">User</option>
          <option value="Driver">Driver</option>
        </select><br />
        <label>Image:</label>
        <input type="file" name="Image" /><br />
        <input type="submit" value="Sign up" />
      </form>
    </div>
  );
}

export default SignUp;
