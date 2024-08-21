import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import GoogleLoginButton from "../Components/LoginGoogle";
import { loginUser, googleLogin } from "../api.js";
import "../Style/style.css";

function Login() {
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);

    const login = {
      username: formData.get('Username'),
      password: formData.get('Password'),
    };

    try {
      const response = await loginUser(login);
      const user = response.data;
      const imageBase64 = await getBase64String(user.imageBytes);

      // Save user data and image to localStorage
      const userWithBase64Image = { ...user, imageBase64 };
      localStorage.setItem('user', JSON.stringify(userWithBase64Image));

      navigate('/Home');
    } catch (error) {
      console.error('Error logging in:', error);
    }
  };

  const getBase64String = async (byteArray) => {
    const blob = new Blob([byteArray], { type: 'image/jpeg' });
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(blob);
      reader.onloadend = () => resolve(reader.result);
      reader.onerror = reject;
    });
  };

  const handleGoogleLoginSuccess = async (response) => {
    const { tokenId } = response;

    try {
      const serverResponse = await googleLogin(tokenId);
      const { user, token } = serverResponse.data;

      localStorage.setItem('token', token);
      localStorage.setItem('user', JSON.stringify(user));

      navigate('/Home');
    } catch (error) {
      console.error('Google Login Error:', error);
    }
  };

  const handleGoogleLoginFailure = (response) => {
    console.error('Google Login Failed:', response);
  };

  const handleSignUpClick = () => {
    navigate('/SignUp');
  };

  return (
    <div className="SignUpBg">
      <label>Log in</label>
      <form className="forma" onSubmit={handleSubmit}>
        <input type="text" name="Username" placeholder="Username" /><br />
        <input type="password" name="Password" placeholder="Password" /><br />
        <input type="submit" value="Login" />
      </form>

      <GoogleLoginButton 
        onSuccess={handleGoogleLoginSuccess}
        onFailure={handleGoogleLoginFailure}
      />

      <button onClick={handleSignUpClick}>Sign Up</button>
    </div>
  );
}

export default Login;
