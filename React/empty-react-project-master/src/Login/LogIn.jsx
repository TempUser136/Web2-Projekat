
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
      const response = await axios.post(`http://localhost:8613/user/Login/${1}`, login);
      const user = response.data;
      //localStorage.setItem('token', token);
      const imageBase64 = await getBase64String(user.imageBytes);

    // Save user data and image to localStorage
    const userWithBase64Image = { ...user, imageBase64 };
    localStorage.setItem('user', JSON.stringify(userWithBase64Image));

      console.log('User logged in:', response.data);
      console.log('Storage:', localStorage.getItem('user'));

      

      navigate('/Home');
    } catch (error) {
      console.error('Error logging in:', error);
    }
  };

// Helper function to convert byte array to Base64 string
const getBase64String = async (byteArray) => {
  const blob = new Blob([byteArray], { type: 'image/jpeg' }); // Change MIME type if needed
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(blob);
    reader.onloadend = () => resolve(reader.result);
    reader.onerror = reject;
  });
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