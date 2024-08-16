import React, { useEffect } from 'react';

function GoogleLoginButton() {
  useEffect(() => {
    /* global google */
    google.accounts.id.initialize({
      client_id: '277563917135-m4ve2h7mokmpmcfks4s87pd2llue8k6r.apps.googleusercontent.com',
      callback: handleCredentialResponse,
    });

    google.accounts.id.renderButton(
      document.getElementById('googleSignInButton'),
      { theme: 'outline', size: 'large' }
    );
  }, []);

  const handleCredentialResponse = (response) => {
    console.log('Encoded JWT ID token: ' + response.credential);
    // You can now send this JWT to your server to validate and sign in the user.
  };

  return (
    <div id="googleSignInButton"></div>
  );
}

export default GoogleLoginButton;
