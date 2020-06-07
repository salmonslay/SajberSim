<?php
// REQUESTPASSWORDRESET.PHP
// INPUT: email via _POST
// FUNCTIONALITY: Tries to find an account with the specified email and then creates a one-time url link for RESETPASSWORD.HTML
// OUTPUT: "success" if all went according to plan or an error message containing "error"

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

$from = $emailAccount;

// Get the id
$email=$_POST['email'];

// To protect MySQL injection (more detail about MySQL injection)
$email = stripslashes($email);
$email = mysqli_real_escape_string($link, $email);
$email = str_replace('%40', '@', $email );
$email = str_replace('%2b', '+', $email );

// Check If the specified username already exists,
$query="SELECT id, username, email FROM accounts WHERE email = '$email'";
$result= try_mysql_query($link, $query);

// If it doesn't
if( mysqli_num_rows($result) ==0){
die( "MySQL error: Could not find the specified email - " . $email);}

$temp = mysqli_fetch_array($result);

$id = $temp['id'];
$username = $temp['username'];

// Generate a random hash and sha1 it
$hash = sha1(uniqid(rand(), true));

// First check if it exists
$query="SELECT accountid FROM passwordreset WHERE accountid = '$id'";
$result= try_mysql_query($link, $query);

// If it does,
if( mysqli_num_rows($result) != 0){

	// Update it with the new hash
	$query="UPDATE passwordreset SET hash = '$hash' WHERE accountid = '$id'";

        try_mysql_query($link, $query);

}
// If it doesn't
else {

	// Insert it
	$query="INSERT INTO passwordreset VALUES ('$id', '$hash')";

	try_mysql_query($link, $query);

}

$to       = $temp['email'];

// Re-Insert slashes
$resetLink    = $phpScriptsLocation . '/resetpassword.html?id=' . $id . '&hash=' . $hash;

$htmlContent = '
<!doctype html>
<html>

<head>
  <meta name="viewport" content="width=device-width" />
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
  <title>Simple Transactional Email</title>
  <style>
    /* -------------------------------------
          GLOBAL RESETS
      ------------------------------------- */

    /*All the styling goes here*/

    img {
      border: none;
      -ms-interpolation-mode: bicubic;
      max-width: 100%;
    }

    body {
      background-color: #f6f6f6;
      font-family: sans-serif;
      -webkit-font-smoothing: antialiased;
      font-size: 14px;
      line-height: 1.4;
      margin: 0;
      padding: 0;
      -ms-text-size-adjust: 100%;
      -webkit-text-size-adjust: 100%;
    }

    table {
      border-collapse: separate;
      mso-table-lspace: 0pt;
      mso-table-rspace: 0pt;
      width: 100%;
    }

    table td {
      font-family: sans-serif;
      font-size: 14px;
      vertical-align: top;
    }

    /* -------------------------------------
          BODY & CONTAINER
      ------------------------------------- */

    .body {
      background-color: #f6f6f6;
      width: 100%;
    }

    /* Set a max-width, and make it display as block so it will automatically stretch to that width, but will also shrink down on a phone or something */
    .container {
      display: block;
      margin: 0 auto !important;
      /* makes it centered */
      max-width: 580px;
      padding: 10px;
      width: 580px;
    }

    /* This should also be a block element, so that it will fill 100% of the .container */
    .content {
      box-sizing: border-box;
      display: block;
      margin: 0 auto;
      max-width: 580px;
      padding: 10px;
    }

    /* -------------------------------------
          HEADER, FOOTER, MAIN
      ------------------------------------- */
    .main {
      background: #ffffff;
      border-radius: 3px;
      width: 100%;
    }

    .wrapper {
      box-sizing: border-box;
      padding: 20px;
    }

    .content-block {
      padding-bottom: 10px;
      padding-top: 10px;
    }

    .footer {
      clear: both;
      margin-top: 10px;
      text-align: center;
      width: 100%;
    }

    .footer td,
    .footer p,
    .footer span,
    .footer a {
      color: #999999;
      font-size: 12px;
      text-align: center;
    }

    /* -------------------------------------
          TYPOGRAPHY
      ------------------------------------- */
    h1,
    h2,
    h3,
    h4 {
      color: #000000;
      font-family: sans-serif;
      font-weight: 400;
      line-height: 1.4;
      margin: 0;
      margin-bottom: 30px;
    }

    h1 {
      font-size: 35px;
      font-weight: 300;
      text-align: center;
      text-transform: capitalize;
    }

    p,
    ul,
    ol {
      font-family: sans-serif;
      font-size: 14px;
      font-weight: normal;
      margin: 0;
      margin-bottom: 15px;
    }

    p li,
    ul li,
    ol li {
      list-style-position: inside;
      margin-left: 5px;
    }

    a {
      color: #3498db;
      text-decoration: underline;
    }

    /* -------------------------------------
          BUTTONS
      ------------------------------------- */
    .btn {
      box-sizing: border-box;
      width: 100%;
    }

    .btn>tbody>tr>td {
      padding-bottom: 15px;
    }

    .btn table {
      width: auto;
    }

    .btn table td {
      background-color: #ffffff;
      border-radius: 5px;
      text-align: center;
    }

    .btn a {
      background-color: #ffffff;
      border: solid 1px #3498db;
      border-radius: 5px;
      box-sizing: border-box;
      color: #3498db;
      cursor: pointer;
      display: inline-block;
      font-size: 14px;
      font-weight: bold;
      margin: 0;
      padding: 12px 25px;
      text-decoration: none;
      text-transform: capitalize;
    }

    .btn-primary table td {
      background-color: #3498db;
    }

    .btn-primary a {
      background-color: #3498db;
      border-color: #3498db;
      color: #ffffff;
    }

    /* -------------------------------------
          OTHER STYLES THAT MIGHT BE USEFUL
      ------------------------------------- */
    .last {
      margin-bottom: 0;
    }

    .first {
      margin-top: 0;
    }

    .align-center {
      text-align: center;
    }

    .align-right {
      text-align: right;
    }

    .align-left {
      text-align: left;
    }

    .clear {
      clear: both;
    }

    .mt0 {
      margin-top: 0;
    }

    .mb0 {
      margin-bottom: 0;
    }

    .preheader {
      color: transparent;
      display: none;
      height: 0;
      max-height: 0;
      max-width: 0;
      opacity: 0;
      overflow: hidden;
      mso-hide: all;
      visibility: hidden;
      width: 0;
    }

    .powered-by a {
      text-decoration: none;
    }

    hr {
      border: 0;
      border-bottom: 1px solid #f6f6f6;
      margin: 20px 0;
    }

    /* -------------------------------------
          RESPONSIVE AND MOBILE FRIENDLY STYLES
      ------------------------------------- */
    @media only screen and (max-width: 620px) {
      table[class=body] h1 {
        font-size: 28px !important;
        margin-bottom: 10px !important;
      }

      table[class=body] p,
      table[class=body] ul,
      table[class=body] ol,
      table[class=body] td,
      table[class=body] span,
      table[class=body] a {
        font-size: 16px !important;
      }

      table[class=body] .wrapper,
      table[class=body] .article {
        padding: 10px !important;
      }

      table[class=body] .content {
        padding: 0 !important;
      }

      table[class=body] .container {
        padding: 0 !important;
        width: 100% !important;
      }

      table[class=body] .main {
        border-left-width: 0 !important;
        border-radius: 0 !important;
        border-right-width: 0 !important;
      }

      table[class=body] .btn table {
        width: 100% !important;
      }

      table[class=body] .btn a {
        width: 100% !important;
      }

      table[class=body] .img-responsive {
        height: auto !important;
        max-width: 100% !important;
        width: auto !important;
      }
    }

    /* -------------------------------------
          PRESERVE THESE STYLES IN THE HEAD
      ------------------------------------- */
    @media all {
      .ExternalClass {
        width: 100%;
      }

      .ExternalClass,
      .ExternalClass p,
      .ExternalClass span,
      .ExternalClass font,
      .ExternalClass td,
      .ExternalClass div {
        line-height: 100%;
      }

      .apple-link a {
        color: inherit !important;
        font-family: inherit !important;
        font-size: inherit !important;
        font-weight: inherit !important;
        line-height: inherit !important;
        text-decoration: none !important;
      }

      #MessageViewBody a {
        color: inherit;
        text-decoration: none;
        font-size: inherit;
        font-family: inherit;
        font-weight: inherit;
        line-height: inherit;
      }

      .btn-primary table td:hover {
        background-color: #34495e !important;
      }

      .btn-primary a:hover {
        background-color: #34495e !important;
        border-color: #34495e !important;
      }
    }
  </style>
</head>

<body class="">
  <span class="preheader">SajberSim password reset</span>
  <table role="presentation" border="0" cellpadding="0" cellspacing="0" class="body">
    <tr>
      <td>&nbsp;</td>
      <td class="container">
        <div class="content">

          <!-- START CENTERED WHITE CONTAINER -->
          <table role="presentation" class="main">

            <!-- START MAIN CONTENT AREA -->
            <tr>
              <td class="wrapper">
                <table role="presentation" border="0" cellpadding="0" cellspacing="0">
                  <tr>
                    <td>
										<img src="https://sajber.me/account/Email/characters.png" alt="characters">
                      <p>Hi '.$username.'!</p>
                      <p>To reset your SajberSim account password click on the following link.</p>
                      <table role="presentation" border="0" cellpadding="0" cellspacing="0" class="btn btn-primary">
                        <tbody>
                          <tr>
                            <td align="left">
                              <table role="presentation" border="0" cellpadding="0" cellspacing="0">
                                <tbody>
                                  <tr>
                                    <td> <a href="'.$resetLink.'" target="_blank">Reset password</a> </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                      <p>If you didn\'t issue a password reset you can ignore this email.</p>
                      <p>- SajberSim</p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>

            <!-- END MAIN CONTENT AREA -->
          </table>
          <!-- END CENTERED WHITE CONTAINER -->

          <!-- START FOOTER -->
          <div class="footer">
            <table role="presentation" border="0" cellpadding="0" cellspacing="0">
              <tr>
                <td class="content-block">
                  <br>Need help? Reply to this email or contact <a href="mailto:help@sajber.me">help@sajber.me</a>
                </td>
              </tr>
            </table>
          </div>
          <!-- END FOOTER -->

        </div>
      </td>
      <td>&nbsp;</td>
    </tr>
  </table>
</body>

</html>';

$subject = 'SajberSim password reset'; //email title

$headers = "MIME-Version: 1.0" . "\r\n";
$headers .= "Content-type:text/html;charset=UTF-8" . "\r\n";

$headers .= 'From: ' . $emailAccount . "\r\n" .
	'Reply-To: help@sajber.me' . "\r\n" .
	'X-Mailer: PHP/' . phpversion();



$to = str_replace('%40', '@', $to );

// Send our email
if ( !mail($to, $subject, $htmlContent , $headers) ){

	// Remove it from confirmation table
	$query="DELETE FROM passwordreset WHERE accountid = '$id' and hash = '$hash'";
	try_mysql_query($link, $query);

	$errorMessage  = "error: Could not send the following email:\n";
	$errorMessage .= "\nFROM: "    . $emailAccount;
	$errorMessage .= "\nTO: "      . $to;
	$errorMessage .= "\nSUBJECT: " . $subject;
	$errorMessage .= "\nMESSAGE: " . $htmlContent;
	$errorMessage .= "\nHEADERS: " . $headers;
	$errorMessage .= "\nAlso deleted the password reset request from 'passwordreset' - the password reset link is now broken.";
	$errorMessage .= "\nTo change that functionality edit REQUESTPASSWORDRESET.PHP at line 111.";
	die($errorMessage);
}

// Successful sent email
$successMessage  = "success - sent the following email\n";
$successMessage .= "\nFROM: "    . $emailAccount;
$successMessage .= "\nTO: "      . $to;
$successMessage .= "\nSUBJECT: " . $subject;
$successMessage .= "\nMESSAGE: " . $htmlContent;
$successMessage .= "\nHEADERS: " . $headers;
echo $successMessage;
exit();


?>
