<?php

// credentials for an user with administrator rights on the required projects
// (if you want it to be able to create new versions when needed)
define( 'MANTIS_USER',	'crashreporter' );
define( 'MANTIS_PWD',	'password' );
	
define( 'MANTIS_URL',	'https://your.mantis.url/' );
define( 'MANTIS_WSDL',	MANTIS_URL . 'api/soap/mantisconnect.php?wsdl' );

// constants for the reports
define( 'BUG_SUMMARY',	'[Exception] ' );
define( 'BUG_CATEGORY',	'Crash' );

// associative array - key should be the custom field name. Only required fields will be set.
define( 'BUG_CUSTOM_FIELDS', array(
));
