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

class Mantis
{
	private $userID;
	private $client;

	public function __construct()
	{
		$this->client = new SoapClient( MANTIS_WSDL );
	}
	
	public function getProject( $proj )
	{
		$projects = $this->client->mc_projects_get_user_accessible( MANTIS_USER, MANTIS_PWD );

		return( $this->findProject( $projects, $proj ));
	}

	private function findProject( $projects, $name )
	{
		$ret = null;

		foreach( $projects as $p ) {

			if( $p->name == $name )
				$ret = $p;
			else if( is_array( $p->subprojects ))
				$ret = $this->findProject( $p->subprojects, $name );

			if( $ret )
				break;
		}

		return( $ret );
	}
	
	public function hasVersion( $projID, $version )
	{
		$vers = $this->client->mc_project_get_versions( MANTIS_USER, MANTIS_PWD, $projID );
		$ret  = false;
	
		foreach( $vers as $v )
			if( $v->name == $version ) {
				$ret = true;
				break;
			}
			
		return( $ret );
	}
	
	public function addVersion( $projID, $version )
	{
		$this->client->mc_project_version_add( MANTIS_USER, MANTIS_PWD,
											   array(
													'name'		  => $version,
													'project_id'  => $projID,
													'description' => $version,
													'released'	  => true,
													'date_order'  => date( DATE_ISO8601 )
											   ));			
	}
	
	public function getRequiredCustomFields( $projID )
	{
		$fields = $this->client->mc_project_get_custom_fields( MANTIS_USER, MANTIS_PWD, $projID );
	
		$fields = array_filter( $fields, function( $i ) { return $i->require_report; } );
		
		return $fields;
	}
	
	public function addIssue( $issue )
	{
		return( $this->client->mc_issue_add( MANTIS_USER, MANTIS_PWD, $issue ));
	}
	
	public function addAttachment( $bugID, $name, $str )
	{
		$this->client->mc_issue_attachment_add( MANTIS_USER, MANTIS_PWD, $bugID,
									  			$name, 'text/plain', $str ); 
	}
}
