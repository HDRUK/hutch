import React from "react";
import { ListItem, Text, UnorderedList } from "@chakra-ui/react";
import { LinkCard } from "../LinkCard";

export const DirectoryLinkCard = () => (
  <LinkCard href="/directory" heading="🌐 Directory Guide">
    <Text>Documentation for using the Directory website.</Text>
    <UnorderedList pl={8}>
      <ListItem>Searching for samples or organisations</ListItem>
      <ListItem>
        Managing Organisation Data (Biobank or Network Administrators)
      </ListItem>
      <ListItem>
        Managing the Directory itself (Directory Administrators)
      </ListItem>
    </UnorderedList>
  </LinkCard>
);

export const ApiLinkCard = () => (
  <LinkCard href="/api" heading="🖥 API Guide">
    <Text>
      Documentation for accessing and consuming the Directory Web API.
    </Text>
    <UnorderedList pl={8}>
      <ListItem>Authentication</ListItem>
      <ListItem>Bulk Data Submission</ListItem>
    </UnorderedList>
  </LinkCard>
);

export const InstallationLinkCard = () => (
  <LinkCard href="/installation" heading="🛠 Installation Guide">
    <Text>
      Documentation for installing and maintaining an instance of the software
      stack.
    </Text>
    <UnorderedList pl={8}>
      <ListItem>Overview of the stack and its requirements</ListItem>
      <ListItem>Installing parts of the stack</ListItem>
      <ListItem>Configuring parts of the stack</ListItem>
    </UnorderedList>
  </LinkCard>
);

export const DeveloperLinkCard = () => (
  <LinkCard href="/dev" heading="👩‍💻 Developer Guide">
    <Text>Documentation for Developers working with the source code.</Text>
    <UnorderedList pl={8}>
      <ListItem>Getting started with the code</ListItem>
      <ListItem>Running parts of the stack locally</ListItem>
      <ListItem>Modifying the codebase and contributing to it</ListItem>
    </UnorderedList>
  </LinkCard>
);
