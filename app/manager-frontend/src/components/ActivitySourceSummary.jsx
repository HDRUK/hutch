import {
  Heading,
  HStack,
  Text,
  LinkBox,
  LinkOverlay,
  Box,
  Divider,
  Center,
  Stack,
  useColorModeValue,
  IconButton,
} from "@chakra-ui/react";
import { FaTrash, FaDesktop } from "react-icons/fa";
import { Link } from "react-router-dom";

export const ActivitySourceSummary = ({
  title,
  href,
  sourceURL,
  collectionId,
  onDelete,
  ...p
}) => (
  <Center py={6}>
    <LinkBox maxW={'800'}>

      <Box
        maxW={'800'}
        w={'full'}
        bg={useColorModeValue('white', 'gray.900')}
        boxShadow={'2xl'}
        rounded={'md'}
        p={6}
        overflow={'hidden'}
        {...p}
        _hover={{
          transform: 'translateY(-2px)',
          boxShadow: 'lg',
        }}

      >
        <LinkOverlay as={Link} to={`${href}`}>
        </LinkOverlay>
        <Stack>
          <Text
            color={'blue.500'}
            textTransform={'uppercase'}
            fontWeight={800}
            fontSize={'sm'}
            letterSpacing={1.1}>Activity Source
          </Text>
          <Divider />
          <IconButton
            h={5}
            w={5}
            icon={<FaTrash />}
            alignSelf={'flex-end'}
            style={{ background: 'transparent' }}
            aria-label='Delete'
            color={'red.500'}
            onClick={onDelete} />

          <Heading
            color={useColorModeValue('gray.700', 'white')}
            fontSize={'2xl'}
            fontFamily={'body'}>
            {title}

          </Heading>

        </Stack>
        <Stack mt={6} direction={'row'} spacing={4} align={'center'}>

          <Stack direction={'column'} spacing={0} fontSize={'sm'} align='center' display={'block'}>

            {sourceURL && (

              <Text fontWeight={'600'} display='inline-flex'>
                <FaDesktop display={'flex'} />
                Host: {sourceURL}
              </Text>
            )}

            {collectionId && (
              <Text fontWeight={'600'}  >
                Resource ID: {collectionId}
              </Text>)}
          </Stack>
        </Stack>
      </Box>
      <HStack>

      </HStack>

    </LinkBox >
  </Center >
);
