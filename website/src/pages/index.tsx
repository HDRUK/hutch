import React, { useEffect, useState } from "react";
import Layout from "@theme/Layout";
import { Heading, Text, SimpleGrid, VStack, HStack } from "@chakra-ui/react";
import { FaBook, FaDownload } from "react-icons/fa";
import { Features } from "@site/src/components/homepage/Features";
import { Funders } from "../components/homepage/Funders";
import LinkButton from "../components/LinkButton";
import HutchLogo from "@site/static/img/hutch_logo-color-white.svg";

const HeroButton = (p) => (
  <LinkButton
    size="xl"
    boxShadow="2xl"
    _hover={{ boxShadow: "dark-lg", top: "-5px" }}
    transition="all ease .2s"
    top="0"
    {...p}
  >
    {p.children || null}
  </LinkButton>
);

const randColor = () =>
  ["orange", "purple", "teal", "cyan", "pink", "blue"][
    Math.floor(Math.random() * 6)
  ];

const RandomFederationBanner = () => {
  const responsiveChildWidths = { sm: "100%", md: "70%" };

  const activities = ["Discovery", "Analysis", "Learning", "Activities"];
  const [activity, setActivity] = useState(0);
  const [color, setColor] = useState(randColor);

  // Basically use a timer to cycle activities 'til the end
  useEffect(() => {
    console.log("effect");

    let timeout: NodeJS.Timeout;
    if (activity < activities.length - 1)
      timeout = setTimeout(() => {
        setActivity(activity + 1);
        setColor(randColor);
      }, 1000);

    return () => clearTimeout(timeout);
  }, [activity]);

  return (
    <VStack spacing={2} w={responsiveChildWidths} minHeight={"150px"}>
      <Heading size="lg">Enabling</Heading>
      <Heading
        size={activity === activities.length - 1 ? "3xl" : "2xl"}
        textAlign="center"
        color={`${color}.200`}
      >
        <Text as="span">Federated {activities[activity]}</Text>
      </Heading>
      <Heading size="lg">in Secure Environments</Heading>
    </VStack>
  );
};

const HeroBanner = () => {
  return (
    <VStack
      bgGradient="radial(circle 1000px at top left, cyan.600, blue.900)"
      py={10}
      spacing={8}
      alignItems="center"
      color="white"
      textShadow="2px 2px 2px rgba(0,0,0,.8)"
    >
      <Heading size="4xl">
        <HutchLogo
          style={{ filter: "drop-shadow(2px 2px 2px rgba(0,0,0,.8))" }}
          role="img"
          width="400px"
        />
      </Heading>

      <RandomFederationBanner />

      <HStack spacing={16}>
        {/* <HeroButton
          colorScheme="green"
          leftIcon={<FaDownload />}
          href="https://github.com/hdruk/hutch/releases"
        >
          Download now
        </HeroButton> */}

        <HeroButton
          colorScheme="blue"
          leftIcon={<FaBook />}
          to="docs/users/getting-started/installation"
        >
          Installation Guide
        </HeroButton>
      </HStack>
    </VStack>
  );
};

const LinkCardSection = () => (
  <VStack p={5} w="100%" align="stretch">
    <Heading fontWeight="medium">📚 Documentation</Heading>
    <SimpleGrid p={8} columns={{ sm: 1, md: 2 }} spacing={8}>
      {/* <DirectoryLinkCard />
      <ApiLinkCard />
      <InstallationLinkCard />
      <DeveloperLinkCard /> */}
    </SimpleGrid>
  </VStack>
);

const Index = () => {
  return (
    <Layout>
      <VStack mb={5} align="stretch" spacing={0}>
        <HeroBanner />

        <Funders />

        <Features />

        {/* <LinkCardSection /> */}
      </VStack>
    </Layout>
  );
};

export default Index;
