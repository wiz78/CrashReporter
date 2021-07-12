<?php
	/*
	 * Copyright 2009, Simone Tellini, http://tellini.info
	 *
	 * Licensed under the Apache License, Version 2.0 (the "License");
	 * you may not use this file except in compliance with the License.
	 * You may obtain a copy of the License at
	 *
	 *     http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 */

	require_once( 'config.php' );
	require_once( 'mantis.php' );

	$issue    = new StdClass;
	$crashlog = $_POST[ 'crashes' ];
	$descr	  = array();
	
	if( !empty( $_POST[ 'email' ] ) && ( $_POST[ 'email' ] != '"" <>' ))
		$descr[] = 'From: ' . $_POST[ 'email' ];

	$descr[] = 'IP: ' . $_SERVER[ 'REMOTE_ADDR' ];
		
	if( !empty( $_POST[ 'comment' ] ))
		$descr[] = "\n" . $_POST[ 'comment' ];

	$issue->summary                = substr( $_POST[ 'summary' ], 0, 128 );
	$issue->severity               = array( 'id' => 70 );
	$issue->category               = BUG_CATEGORY;
	$issue->description            = implode( "\n", $descr );
	$issue->additional_information = $crashlog;
	$issue->priority               = array( 'id' => 10 );
	$issue->status                 = array( 'id' => 10 );
	$issue->reproducibility        = array( 'id' => 70 );
	$issue->resolution             = array( 'id' => 10 );
	$issue->projection             = array( 'id' => 10 );
	$issue->eta                    = array( 'id' => 10 );
	$issue->view_state             = array( 'id' => 50 );
	$issue->version                = $_POST[ 'version' ];

	$sys = $_POST[ 'system' ];

	if( !empty( $sys )) {

		$lines = explode( "\n", $sys );
		$data  = array();

		foreach( $lines as $line ) {

			$p = explode( ' = ', $line );

			$data[ $p[ 0 ] ] = $p[ 1 ];
		}

		$issue->platform = $data[ 'CPU_TYPE' ];
		$issue->os       = $data[ 'OS_NAME' ];
		$issue->os_build = $data[ 'OS_VERSION' ];
	}

	if( empty( $issue->description ))
		$issue->description = 'Crashed.';

	$attachments = array( 'console', 'eventlog', 'preferences', 'exception', 'shell', 'system' );

	try {
		$mantis = new Mantis();
		
		$issue->project = $mantis->getProject( $_REQUEST[ 'project' ] );
	
		if( !$mantis->hasVersion( $issue->project->id, $issue->version ))
			$mantis->addVersion( $issue->project->id, $issue->version );
		
		$customFields = $mantis->getRequiredCustomFields( $issue->project->id );
		
		foreach( $customFields as $field ) {
			
			$name = $field->field->name;
			
			if( isset( BUG_CUSTOM_FIELDS[ $name ] ))
				$field->value = BUG_CUSTOM_FIELDS[ $name ];
			else
				$field->value = 'N/A';
		}
		
		$issue->custom_fields = $customFields;
				
		$id = $mantis->addIssue( $issue );

		foreach( $attachments as $f ) {

			$str = $_POST[ $f ];

			if( !empty( $str ))
				$mantis->addAttachment( $id, $f . '.txt', $str ); 
		}
	}
	catch( Exception $e ) {
		print( 'ERR An error occurred while storing the report' );
	}
