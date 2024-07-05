
import React, { useState } from "react";
import axios from "axios";
import LoginModel from "../Models/Login";
import { useNavigate } from "react-router-dom";

function Login() {

  const navigate = useNavigate();
  const handleSubmit = async (e) => {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);

    const login = new LoginModel(
      formData.get('Username'),
      formData.get('Password'),
   
    );

    try {
      const response = await axios.get('http://localhost:8613/Ride/statefull');
      const token = response.data;
      localStorage.setItem('token', token);
      console.log('User logged in:', response.data);
      navigate('/Home');
    } catch (error) {
      console.error('Error logging in:', error);
    }
  };


  return (
    <div class="SignUpBg" >
      <label>Sign Up</label>
      <form className="forma" onSubmit={handleSubmit}>
    <input type="text" name="Username" placeholder="Username" /><br />
    <input type="password" name="Password" placeholder="Password" /><br />
    <input type="submit" value="Login" />
  </form>
    </div>
  );
}
export default Login;